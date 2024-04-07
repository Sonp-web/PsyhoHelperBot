using Newtonsoft.Json;
using Telegram.Bot.Types;
using File = System.IO.File;

namespace ConsoleApp21;

public class AppSettings
{
    public ChatId ManagerChannelId { get; init; }
    public ChatId SubscribeChannelId { get; init; }
    public string GptPrompt { get; init; }
    public string AgencyName { get; init; }
    
}