using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.EventLog;
using System.Diagnostics;

namespace FileRegisterer
{
    class Program
    {

        static async Task Main(string[] args)
        {
            CreateEventLogFile("MLMFileRegisterer", "MLMFileRegisterer");
            System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureAppConfiguration((context, config) =>
                {
                    // configure the app here.
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                    logging.AddEventLog(new EventLogSettings()
                    {
                        SourceName = "MLMFileRegisterer",
                        LogName = "MLMFileRegisterer",
                        Filter = (x, y) => y >= LogLevel.Debug
                    });
                });
        }

        public static void CreateEventLogFile(string sourceName, string fileName)
        {
            if (!EventLog.SourceExists(sourceName))
            {
                //An event log source should not be created and immediately used.
                //There is a latency time to enable the source, it should be created
                //prior to executing the application that uses the source.
                //Execute this sample a second time to use the new source.
                EventLog.CreateEventSource(sourceName, fileName);
                // The source is created.  Exit the application to allow it to be registered.
            }
        }
    }
}