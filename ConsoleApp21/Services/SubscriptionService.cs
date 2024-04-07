using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ConsoleApp21.Services;

public class SubscriptionService
{
    public ChatId SubscribeChatId { get; set; }
    private ITelegramBotClient _botClient;
    private List<ChatMemberStatus> _allowedStatuses=new()
    {
        ChatMemberStatus.Member,
        ChatMemberStatus.Administrator,
        ChatMemberStatus.Creator
    };

    public SubscriptionService(ITelegramBotClient botClient, ChatId subscribeChatId)
    {
        _botClient = botClient;
        SubscribeChatId = subscribeChatId;
    }

    public async Task<bool> IsSubscribed(long userId)
    {
        ChatMember chatMember;
        try
        {

            chatMember = await _botClient.GetChatMemberAsync(SubscribeChatId, userId);
            if (_allowedStatuses.Contains(chatMember.Status))
            {
                return true;
            }
        }
        catch(ApiRequestException exception)
        {
            Console.WriteLine(exception);
            return false;
        }

        

        return false;

    }
    
}