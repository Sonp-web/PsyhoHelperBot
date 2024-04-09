using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp21.StateMchine.States;

public class WaitingForNameState:ChatStateBase
{
    private readonly UserDataProvider _userDataProvider;
    private readonly ITelegramBotClient _botClient;

    public WaitingForNameState(ChatStateMachine chatStateMachine, ITelegramBotClient botClient,
        UserDataProvider userDataProvider) : base(chatStateMachine)
    {
        _userDataProvider = userDataProvider;
        _botClient = botClient;
    }

    public override async Task HandleMessage(Message message)
    {
        var chatId = message.Chat.Id;
        _userDataProvider.SetUserName(chatId,message.Text);
        _userDataProvider.SetTelegramName(chatId,message.From.Username);
        await _stateMachine.TransitTo<WaitingForPhoneState>(chatId);
    }

    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);
        await _botClient.SafeSendTextMessageAsync(chatId, "Пожалуйста, введите ваше ФИО.");
    }
}