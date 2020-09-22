using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VisitorStatistics.Services.Abstraction;

namespace VisitorStatistics.Services
{
    /// <summary>
    /// The purpose with this timer is to automatically remove visit data from the database based on 
    /// stored deletion dates. The deletion dates are set based on configured deletion days in the
    /// appsettings.json file, which can be configured via the application admin panel "VisitSettings".
    /// </summary>
    public class TimedHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<TimedHostedService> _logger;
        private readonly IServiceScopeFactory _scope;
        private readonly IConfiguration _config;
        private Timer _timer;

        public TimedHostedService(ILogger<TimedHostedService> logger, IConfiguration config, IServiceScopeFactory scope)
        {
            _logger = logger;
            _config = config;
            _scope = scope;
        }

        /// <summary>
        /// Task to set and start the timer on a daily basis to check for matching
        /// deletion dates to delete visitor statistics from the database.
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service running.");

            _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromDays(1));

            return Task.CompletedTask;
        }

        /// <summary>
        /// This is the callback method to trigger the events we want by the timer.
        /// </summary>
        /// <param name="state"></param>
        private void DoWork(object state)
        {
            // Create service scope to solve necessary dependency to IVisitorHandler:
            var scope = _scope.CreateScope();
            var handler = scope.ServiceProvider.GetService<IVisitorHandler>();

            handler.DeleteVisitors();

            _logger.LogInformation("DeleteVisitorsTimer was executed.");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Timed Hosted Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }

    } // End class.
} // End namespace.
