using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp21.StateMchine.States;

public class StartState:ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    private string _agencyName;
    
    public StartState(ChatStateMachine stateMachine,ITelegramBotClient botClient,string agencyName) : base(stateMachine)
    {
        _botClient = botClient;
        _agencyName = agencyName;
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }

    public async Task SendGreeting(long chatId)
    {
        var message = "Приветствую!\n" +
                      $"Я первый психологический бот с искусственным интелектом, созданный агенством психологической поддержки *{_agencyName}*\n" +
                      "Я могу оказать Вам мгновенную первичную консультацию по любому интересующему Вас вопросу или передать Ваши данные нашему специалисту для более детальной консультации.";
        
        var inlineKeyboardMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("",GlobalData.QUESTION),
                InlineKeyboardButton.WithCallbackData("",GlobalData.SPECIALIST), 
            }
        });
        await _botClient.SendTextMessageAsync(chatId: chatId, message,replyMarkup:inlineKeyboardMarkup);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }

    public  override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);
        await SendGreeting(chatId);
    }
}