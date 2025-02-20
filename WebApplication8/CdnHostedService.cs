namespace WebApplication8;

public class CdnHostedService : IHostedService
{
    private readonly IBackgroundTaskManager _backgroundTaskManager;
    private Task _executingTask;
    private CancellationTokenSource _combinedCts;

    public CdnHostedService(IBackgroundTaskManager backgroundTaskManager)
    {
        _backgroundTaskManager = backgroundTaskManager;
        _backgroundTaskManager.CancellationTokenUpdated += OnCancellationTokenUpdated;
    }

    private void OnCancellationTokenUpdated()
    {
        // Обновляем комбинированный токен отмены
        _combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
            _backgroundTaskManager.GetCancellationToken()
        );

        // Если задача уже завершилась, перезапускаем её
        if (_executingTask?.IsCompleted == true)
        {
            _executingTask = ExecuteAsync(_combinedCts.Token);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            _backgroundTaskManager.GetCancellationToken()
        );
        _executingTask = ExecuteAsync(_combinedCts.Token);
        return Task.CompletedTask;
    }

    private async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (_backgroundTaskManager.TryDequeue(out var message))
            {
                Console.WriteLine($"Сообщение из очереди: {message}");
            }
            await Task.Delay(5000, stoppingToken); // Асинхронная задержка
        }
        Console.WriteLine("Фоновая задача завершена.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_executingTask == null)
        {
            return;
        }
        try
        {
            // Отправляем сигнал отмены
            _combinedCts.Cancel();
        }
        finally
        {
            // Ожидаем завершения задачи
            await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
        }
    }
}