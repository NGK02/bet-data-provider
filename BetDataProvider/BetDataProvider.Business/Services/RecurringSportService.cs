using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.DataAccess.Repositories.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.Business.Services
{
    public class RecurringSportService : BackgroundService
    {
        private readonly TimeSpan _feedPullDelay = TimeSpan.FromSeconds(60);

        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RecurringSportService> _logger;

        public RecurringSportService(IServiceScopeFactory serviceScopeFactory, ILogger<RecurringSportService> logger)
        {
            this._serviceScopeFactory = serviceScopeFactory;
            this._logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int counter = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                var watch = Stopwatch.StartNew();

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var xmlHandler = scope.ServiceProvider.GetRequiredService<IExternalDataService>();
                    var sportData = await xmlHandler.GetAndParseXmlDataAsync();
                    xmlHandler.SaveSportData(sportData);
                }

                watch.Stop();
                var elapsedMs = watch.ElapsedMilliseconds;
                counter += 1;
                _logger.LogInformation($"Cycle {counter} completed in {elapsedMs}ms");

                await Task.Delay(_feedPullDelay, stoppingToken);
            }
        }
    }
}
