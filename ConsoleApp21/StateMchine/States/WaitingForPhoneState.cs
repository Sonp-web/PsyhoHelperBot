using System.Text.RegularExpressions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace ConsoleApp21.StateMchine.States;

public class WaitingForPhoneState:ChatStateBase
{
    private readonly UserDataProvider _userDataProvider;
    private readonly ITelegramBotClient _botClient;

    public WaitingForPhoneState(ChatStateMachine chatStateMachine, ITelegramBotClient botClient,
        UserDataProvider userDataProvide) : base(chatStateMachine)
    {
        _userDataProvider = userDataProvide;
        _botClient = botClient;
    }

    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);
        await _botClient.SafeSendTextMessageAsync(chatId, "Пожалуйста, введите ваш номер телефона.");
    }

    public override async Task HandleMessage(Message message)
    {
        var chatId = message.Chat.Id;
        var messageText = message.Text;

        string pattern = @"^\+\d{1,3}\(\d{2,3}\)\d{7}$";
        if (!Regex.IsMatch(messageText, pattern, RegexOptions.IgnoreCase))
        {
            await _botClient.SafeSendTextMessageAsync(chatId,
                "Номер телефона введен некорректно.\\nПожалуйста, введите номер без букв.");
            return;
        }
        
        _userDataProvider.SetUserPhone(chatId,messageText);
        await _stateMachine.TransitTo<UserDataSubmissionState>(chatId);
        
    }
}