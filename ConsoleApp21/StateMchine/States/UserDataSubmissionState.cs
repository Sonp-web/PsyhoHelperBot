using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp21.StateMchine.States;

public class UserDataSubmissionState:ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    private readonly UserDataProvider _userDataProvider;
    private readonly ChatId _managerChannelId;

    public UserDataSubmissionState(ChatStateMachine chatStateMachine, ITelegramBotClient botClient,
        UserDataProvider userDataProvider, ChatId managerChannelId) : base(chatStateMachine)
    {
        _botClient = botClient;
        _userDataProvider = userDataProvider;
        _managerChannelId = managerChannelId;
    }

    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);
        await _botClient.SafeSendTextMessageAsync(chatId,
            "Спасибо за обращение. Данные переданы нашему специалисту, он свяжется с Вами в скором времени.");
        await _stateMachine.TransitTo<IdleState>(chatId);
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }

    public async Task SendUserInfoToManager(long chatId)
    {
        var userData = _userDataProvider.GetUserData(chatId);
        var message = BuildUserInfo(userData);
        await _botClient.SafeSendTextMessageAsync(_managerChannelId, message);
        _userDataProvider.ClearUserData(chatId);
    }

    public override async Task OnExit(long chatId)
    {
        await base.OnExit(chatId);
        await SendUserInfoToManager(chatId);
    }

    public string BuildUserInfo(UserData userProfile)
    {
        var message = "Информация о клиенте:\n" +
                      $"ФИО: {userProfile.Name}\n" +
                      $"Номер телефона: {userProfile.Phone}\n";
        if (!string.IsNullOrEmpty(userProfile.Telegram))
        {
            message += $"Telegram: @{userProfile.Telegram}\n";
        }

        if (!string.IsNullOrEmpty(userProfile.LastQuestion))
        {
            message += $"Последний отправленный вопрос боту: {userProfile.LastQuestion}";
        }

        return message;
    }
}