using Entitites.Models;
using FileRegisterer.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FileRegisterer
{
    class TokenService
    {
        private static string bearerToken { get; set; }
        private static DateTime tokenTime { get; set; } = DateTime.MinValue;
        private ApiKey multiClientApiKey;
        private readonly string tokenUrl;

        public TokenService(ApiKey multiClientApiKey, string tokenUrl)
        {
            this.multiClientApiKey = multiClientApiKey;
            this.tokenUrl = tokenUrl;
        }

        public async Task<string> GetTokenAsync()
        {
            if (DateTime.UtcNow.CompareTo(tokenTime) >= 0)
            {
                tokenTime = DateTime.UtcNow.AddHours(2);
                bearerToken = await GetBearerTokenOfCompany(tokenUrl, multiClientApiKey);
            }

            return bearerToken;
        }

        private async Task<string> GetBearerTokenOfCompany(string url, ApiKey apiKey)
        {
            TokenResult result = null;

            if (string.IsNullOrWhiteSpace(url)) throw new ArgumentNullException(nameof(url));
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey));
            if (string.IsNullOrWhiteSpace(apiKey.ClientId)) throw new ArgumentNullException($"CompanyId: {apiKey.CompanyID} ,{nameof(apiKey.ClientId)}");
            if (string.IsNullOrWhiteSpace(apiKey.ClientSecret)) throw new ArgumentNullException($"CompanyId: {apiKey.CompanyID} ,{nameof(apiKey.ClientSecret)}");

            var handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                SslProtocols = SslProtocols.Tls12
            };

            using (var httpClient = new HttpClient(handler))
            {
                List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();
                keyValues.Add(new KeyValuePair<string, string>("client_id", apiKey.ClientId));
                keyValues.Add(new KeyValuePair<string, string>("client_secret", apiKey.ClientSecret));
                keyValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                var body = new FormUrlEncodedContent(keyValues);

                using (var response = await httpClient.PostAsync(url, body))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        result = JsonConvert.DeserializeObject<TokenResult>(apiResponse);
                    }
                    else
                    {
                        throw new Exception(response.ReasonPhrase);
                    }
                }
            }

            return result.AccessToken;
        }
    }
}