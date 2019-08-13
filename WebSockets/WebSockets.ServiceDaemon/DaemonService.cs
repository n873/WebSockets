using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WebSockets.ServiceDaemon
{
    public abstract class DaemonService : IHostedService, IDisposable
    {
        private readonly ILogger _logger;
        private readonly IOptions<DaemonConfig> _config;
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        
        protected abstract Task ExecuteAsync(CancellationToken stoppingToken);

        
        public DaemonService(ILogger<DaemonService> logger, IOptions<DaemonConfig> config)
        {
            _logger = logger;
            _config = config;
        }

        public virtual Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting daemon: " + _config.Value.DaemonName);
            _executingTask = ExecuteAsync(_stoppingCts.Token);
            return _executingTask.IsCompleted ? _executingTask : Task.CompletedTask;
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping daemon.");
            if (_executingTask == null)
                return;
            
            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing....");
            _stoppingCts.Cancel();
        }
    }
}