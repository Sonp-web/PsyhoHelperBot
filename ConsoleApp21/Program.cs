
using ConsoleApp21.Services;
using Newtonsoft.Json;
using Telegram.Bot;

namespace ConsoleApp21;

class Program
{
    private const string APP_SETTINGS_JSON_PATH = "C:\\Users\\Егор\\RiderProjects\\ConsoleApp21\\ConsoleApp21\\AppSettings\\app_settings.json";
    private const string SECRETS_JSON_PATH = "C:\\Users\\Егор\\RiderProjects\\ConsoleApp21\\ConsoleApp21\\AppSettings\\secrets.json";
    
    static async Task Main(string[] args)
    {
        var secretsJson = File.ReadAllText(SECRETS_JSON_PATH);
        var secrets = JsonConvert.DeserializeObject<Secrets>(secretsJson);

        var settingsJson = File.ReadAllText(APP_SETTINGS_JSON_PATH);
        var settings = JsonConvert.DeserializeObject<AppSettings>(settingsJson);

        var botClient = new TelegramBotClient(secrets.ApiKeys.TelegramKey);
        var subscriptionService = new SubscriptionService(botClient, settings.SubscribeChannelId);
        
        var telegramBotController = new TelegramBotController(botClient,subscriptionService);
        telegramBotController.StartBot();
        await Task.Delay(Timeout.Infinite);

        
        
    }
}

