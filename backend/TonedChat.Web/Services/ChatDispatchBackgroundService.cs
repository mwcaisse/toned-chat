namespace TonedChat.Web.Services;

public class ChatDispatchBackgroundService : BackgroundService
{

    private readonly ChatService _chatService;
    
    public ChatDispatchBackgroundService(ChatService chatService)
    {
        _chatService = chatService;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
        await _chatService.SendMessagesWork(stoppingToken);
    }
}