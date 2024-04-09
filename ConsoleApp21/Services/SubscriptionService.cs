using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace ConsoleApp21.Services;

public class SubscriptionService
{
    public ChatId SubscribeChatId { get; set; }
    public ChatId ManagerChannelId { get; set; }
    private ITelegramBotClient _botClient;
    private List<ChatMemberStatus> _allowedStatuses=new()
    {
        ChatMemberStatus.Member,
        ChatMemberStatus.Administrator,
        ChatMemberStatus.Creator
    };

    public SubscriptionService(ITelegramBotClient botClient, ChatId subscribeChatId,ChatId managerChannelId)
    {
        _botClient = botClient;
        SubscribeChatId = subscribeChatId;
        ManagerChannelId = managerChannelId;
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