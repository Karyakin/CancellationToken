using Microsoft.AspNetCore.Mvc;

namespace CancellationToken;

[ApiController]
public class TestController : ControllerBase
{
    private readonly IBackgroundTaskManager _backgroundTaskManager;

    public TestController(IBackgroundTaskManager backgroundTaskManager)
    {
        _backgroundTaskManager = backgroundTaskManager;
    }

    [HttpGet("/EnqueueMessage")]
    public async Task<IActionResult> EnqueueMessage(string token, System.Threading.CancellationToken cancellationToken)
    {
        await _backgroundTaskManager.EnqueueMessageAsync(token, cancellationToken);
        return Ok("Сообщение добавлено в очередь!");
    }

    [HttpGet("/cancel")]
    public async Task<IActionResult> Cancel(System.Threading.CancellationToken cancellationToken)
    {
        await _backgroundTaskManager.CancelAsync(cancellationToken);
        return Ok("Фоновая задача отменена.");
    }

    [HttpGet("/restart")]
    public async Task<IActionResult> Restart(System.Threading.CancellationToken cancellationToken)
    {
        await _backgroundTaskManager.RestartAsync(cancellationToken);
        return Ok("Фоновая задача перезапущена.");
    }
}