using System.Collections.Concurrent;

namespace WebApplication8;

using System;
using System.Threading;

using System.Threading;
using System.Threading.Tasks;

using System.Threading;
using System.Threading.Tasks;

public interface IBackgroundTaskManager
{
    Task EnqueueMessageAsync(string message, CancellationToken cancellationToken = default);
    Task CancelAsync(CancellationToken cancellationToken = default);
    Task RestartAsync(CancellationToken cancellationToken = default);
    bool TryDequeue(out string message);
    CancellationToken GetCancellationToken();
}

public class BackgroundTaskManager : IBackgroundTaskManager
{
    private readonly ConcurrentQueue<string> _messageQueue = new ConcurrentQueue<string>();
    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public async Task EnqueueMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        _messageQueue.Enqueue(message);
        await Task.CompletedTask; // Имитация асинхронной операции
    }

    public async Task CancelAsync(CancellationToken cancellationToken = default)
    {
        _cancellationTokenSource.Cancel();
        await Task.CompletedTask; // Имитация асинхронной операции
    }

    public async Task RestartAsync(CancellationToken cancellationToken = default)
    {
        // Сбрасываем CancellationTokenSource
        _cancellationTokenSource.Dispose();
        _cancellationTokenSource = new CancellationTokenSource();
        await Task.CompletedTask; // Имитация асинхронной операции
    }

    public bool TryDequeue(out string message)
    {
        return _messageQueue.TryDequeue(out message);
    }

    public CancellationToken GetCancellationToken()
    {
        return _cancellationTokenSource.Token;
    }
}