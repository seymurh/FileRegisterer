using FileRegisterer.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;
using Communication = FileRegisterer.Models.Communication;

namespace FileRegisterer
{
    public class FileTransferService
    {
        private readonly string baseUrl;
        private readonly string tokenUrl;
        private readonly ILogger<Worker> _logger;
        private readonly int _requestCount;
        private readonly TokenService tokenService;
        private readonly string clientId;

        public FileTransferService(string baseUrl, string key, string tokenUrl, ILogger<Worker> logger, int requestCount)
        {
            this.baseUrl = baseUrl;
            this.tokenUrl = tokenUrl;
            _logger = logger;
            _requestCount = requestCount;

            try
            {
                using (var context = new DefinitionContext())
                {
                    tokenService = new TokenService(multiClientApiKey, tokenUrl);
                }
            }
            catch(Exception ex)
            {
                _logger.LogWarning(new EventId(5), $"FileTransferService: Constructor {ex.Message}");
            }
        }

        public async Task RegisterAllCompanyFiles()
        {
            try
            {
                using (var context = new DefinitionContext())
                {
                    List<int> companyIds = context.MigrationStatuses
                        .GroupBy(m => m.CompanyID).Select(g => g.Key).ToList();
                    

                    foreach (var companyId in companyIds)
                    {
                        try
                        {
                            CompanyRequest company = new CompanyRequest
                            {
                                RaetCompanyId = (await context.Companies.SingleAsync(c => c.Id == companyId)).RaetCompanyId,
                                CompanyId = companyId
                            };

                            await ExecuteCaseManagementPreventionRequests(company);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(new EventId(1), $"RegisterAllCompanyFiles: \n {ex.Message} \n {ex.InnerException?.Message}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(new EventId(2), $"RegisterAllCompanyFiles: Service stopped because of Exception {ex.Message}");
            }
        }

        public async Task ExecuteCaseManagementPreventionRequests(CompanyRequest company)
        {
            using (var customerContext = new CustomerContext(company.CompanyId))
            {
                List<RequestLog> requestLogs = await customerContext.RequestLogs.Where(r => r.ServiceMethod == ServiceNames.GetCaseManagementPrevention && r.Status == (int)RequestLogStatus.NotStarted).Take(_requestCount).ToListAsync();
                foreach (var log in requestLogs)
                {
                    var result = await GetCaseManagementPrevention(company, log);
                    await QueueCasemanagementPreventionFiles(result, customerContext);
                    log.ExecutedDate = DateTime.UtcNow;
                    customerContext.SaveChanges();
                }
            }
        }

        public async Task<CasemanagementPreventionResult> GetCaseManagementPrevention(CompanyRequest company, RequestLog requestLog)
        {
            string url = $"{baseUrl}CaseManagementPrevention?personalCode={requestLog.PersonalCode}";
            return await GetRequest<CasemanagementPreventionResult>(company, requestLog, url);
        }

        public async Task<T> GetRequest<T>(CompanyRequest company, RequestLog requestLog, string getUrlWithParams) where T : class
        {
            if (string.IsNullOrWhiteSpace(getUrlWithParams)) throw new ArgumentNullException(nameof(getUrlWithParams));
            if (company == null) throw new ArgumentNullException(nameof(company));

            string bearerToken = await tokenService.GetTokenAsync();
            if (string.IsNullOrWhiteSpace(bearerToken)) throw new ArgumentNullException($"Bearer token is null or empty");

            T reservationList = null;
            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                SslProtocols = SslProtocols.Tls12
            };

            try
            {
                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.Add("FileRegisterer", "true");
                    httpClient.DefaultRequestHeaders.Add("x-client-id", clientId);
                    httpClient.DefaultRequestHeaders.Add("mlm-company-id", company.CompanyId.ToString());
                    httpClient.DefaultRequestHeaders.Add("raet-scope-tenant-id", company.RaetCompanyId);
                    
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

                    using (var response = await httpClient.GetAsync(getUrlWithParams))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            string apiResponse = await response.Content.ReadAsStringAsync();
                            reservationList = JsonConvert.DeserializeObject<T>(apiResponse);
                            requestLog.Status = (int)RequestLogStatus.Completed;
                        }
                        else if (response.StatusCode == HttpStatusCode.TooManyRequests)
                        {
                            requestLog.ExecutionError = "TooManyRequests";
                        }
                        else
                        {
                            requestLog.Status = (int)RequestLogStatus.ExecutedWithErrors;
                            requestLog.ExecutionError = response.ReasonPhrase;
                        }
                    }
                }
            }
            catch(Exception ex)
            {
                _logger.LogWarning(new EventId(3), $"GetRequest {ex.Message}");
            }

            return reservationList;
        }
    }
}