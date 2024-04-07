using ConsoleApp21.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp21;

public class TelegramBotController
{
    private readonly ITelegramBotClient _botClient;
    private readonly SubscriptionService _subscriptionService;

    public TelegramBotController(ITelegramBotClient botClient,SubscriptionService subscriptionService)
    {
        _botClient = botClient;
        _subscriptionService = subscriptionService;
    }

    public void StartBot()
    {
        using var cts = new CancellationTokenSource();
        
        
        var receiverOptions = new ReceiverOptions() //настройка приема обновлений
        {
            AllowedUpdates = new [] { UpdateType.Message ,UpdateType.CallbackQuery}
        };
        CreateCommandsKeyboard().WaitAsync(cts.Token);
        //
        _botClient.StartReceiving(
            HandleUpdateAsync,
            HandleErrorAsync,receiverOptions,cts.Token);
       
        
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update,CancellationToken cancellationToken)
    {
        if (update.Message==null && update.Type ==null )
        {
            return;
        }

        var message = update.Message;
        var callbackQuery = update.CallbackQuery;
        if (message != null && message.Type != MessageType.Text)
        {
            return;
        }

        long userId = 0;
        int messageId=0;
        string messageText = null;

        if (message != null)
        {
            userId = message.From.Id;
            messageId = message.MessageId;
            messageText = message.Text;
        }else if (callbackQuery != null)
        {
            userId = callbackQuery.From.Id;
            messageId = callbackQuery.Message.MessageId;
            messageText = callbackQuery.Data;
        }

        string response;
        if (messageText == GlobalData.CHECK_SUBSCRIPTION)
        {
            if (!await _subscriptionService.IsSubscribed(userId))
            {
                response = "Для продолжения работы бота, Вам необходимо быть подписчиком канала";
                await DeleteMessageAsync(userId, messageId, cancellationToken);
                await SendSubscriptionMessage(userId, response);
                return;
            }

            await DeleteMessageAsync(userId, messageId, cancellationToken);

        }
            if (await _subscriptionService.IsSubscribed(userId))
            {
                response = "Вы подписаны";
                await _botClient.SendTextMessageAsync(userId, response, cancellationToken: cancellationToken);

            }
            else
            {
                response = "Безлимитное использование бота доступно подписчикам канала";
                await SendSubscriptionMessage(userId, response);
            }
        

    }

    private async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine(exception);
    }

    private async Task CreateCommandsKeyboard()
    {
        await _botClient.DeleteMyCommandsAsync();

        var commands = new[]
        {
            new BotCommand() { Command = GlobalData.START, Description = "Начать работу" }
        };
        await _botClient.SetMyCommandsAsync(commands);
    }

   private async Task SendSubscriptionMessage(long chatId, string response)
{
    try
    {
        var channelInfo = await _botClient.GetChatAsync(_subscriptionService.SubscribeChatId);

        var channelName = channelInfo.Title.EscapeMarkdownV2();
        var channelUsername = channelInfo.Username;
        var channelLink = $"[{channelName}](https://t.me/{channelUsername})";

        var subscriptionMessage = $"{response} {channelLink}";

        var keyboardButton = new InlineKeyboardButton("Проверить подписку")
        {
            CallbackData = GlobalData.CHECK_SUBSCRIPTION
        };

        var inlineKeyboardMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                keyboardButton
            }
        });

        await _botClient.SendTextMessageAsync(chatId, subscriptionMessage,
            replyMarkup: inlineKeyboardMarkup);
    }
    catch (ApiRequestException exception)
    {
        Console.WriteLine(exception);
    }
}
    private async Task DeleteMessageAsync(long chatId,int messageId, CancellationToken cancellationToken)
    {
        try
        {
            await _botClient.DeleteMessageAsync(chatId,messageId,cancellationToken);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
}