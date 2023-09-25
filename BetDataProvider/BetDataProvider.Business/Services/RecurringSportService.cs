using BetDataProvider.Business.Services.Contracts;
using BetDataProvider.DataAccess.Repositories.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetDataProvider.Business.Services
{
    public class RecurringSportService : BackgroundService, IRecurringSportService
    {
        private readonly TimeSpan _feedPullDelay = TimeSpan.FromSeconds(60);

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RecurringSportService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var xmlHandler = scope.ServiceProvider.GetRequiredService<IXmlHandler>();
                    var sportData = await xmlHandler.GetAndParseXmlDataAsync();
                    xmlHandler.SaveSportData(sportData);
                }
                await Task.Delay(_feedPullDelay, stoppingToken);
            }
        }
    }
}
