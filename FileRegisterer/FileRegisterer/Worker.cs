using Entitites;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace FileRegisterer
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogWarning($"FileRegisterer started at: {DateTime.UtcNow}");
                string currentDirectory = Directory.GetCurrentDirectory();
                IConfiguration config = new ConfigurationBuilder().SetBasePath(currentDirectory).AddJsonFile("appsettings.json", true, true).Build();
                string baseUrl = config["apiUrl"];
                string key = config["key"];
                string tokenUrl = config["tokenUrl"];
                string startTime = config["startTime"];
                string endTime = config["endTime"];
                string interval = config["interval"];
                int requestCount = int.Parse(config["requestCount"]);
                int min, sec;
                SchedulerService.ParseTime(interval, out min, out sec);
                SchedulerService schedulerService = new SchedulerService(min, sec);

                FileTransferService fileTransferService = new FileTransferService(baseUrl, key, tokenUrl, _logger, requestCount);

                schedulerService.Schedule(async () =>
                {
                    if (schedulerService.IsInInterval(startTime, endTime))
                    {
                        await fileTransferService.RegisterAllCompanyFiles();
                    }
                });

                while (!stoppingToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.Message);
            }
        }
    }
}