using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp21.StateMchine.States;

public class DoneState:ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    public DoneState(ChatStateMachine chatStateMachine,ITelegramBotClient botClient) : base(chatStateMachine)
    {
        _botClient = botClient;
    }

    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);
        var keyboardMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Задать вопрос.", GlobalData.QUESTION),
                InlineKeyboardButton.WithCallbackData("Консультация специалиста", GlobalData.SPECIALIST),
            }
        });
        await _botClient.SafeSendTextMessageAsync(chatId,
            "Спасибо, что воспользовались нашими услугами.\n" + "Если потребуется, я могу оказать Вам мгновенную первичную консультацию по любому интересующему\n" + " Вас вопросу, либо передать Ваши данные нашему специалисту для более детальной консультации.\n",replyMarkup:keyboardMarkup);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;;
    }
}