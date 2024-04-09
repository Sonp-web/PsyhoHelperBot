using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ConsoleApp21.StateMchine.States;

public class QuestionState:ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    private readonly IChatGptService _chatGptService;
    private readonly UserDataProvider _userDataProvider;

    public QuestionState(ChatStateMachine chatStateMachine,ITelegramBotClient botClient, IChatGptService chatGptService,UserDataProvider userDataProvider):base(chatStateMachine)
    {
        _botClient = botClient;
        _chatGptService = chatGptService;
        _userDataProvider = userDataProvider;
    }

    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);
        await _botClient.SafeSendTextMessageAsync(chatId,
            "Задайте Ваш вопрос. На ответ мне может потребоваться около минуты.");
    }

    public async override Task HandleMessage(Message message)
    {
        var chatId = message.From.Id;
        var messageText = message.Text;
        _userDataProvider.SaveLastQuestion(chatId,messageText);
        await _botClient.SafeSendChatActionAsync(chatId, ChatAction.Typing);

        var answer = await _chatGptService.GetAnswerFromGpt(messageText);
        if (answer == null)
        {
            await HandleNullAnswer(chatId);
            return;
        }

        await _botClient.SafeSendTextMessageAsync(chatId, answer);
        var replyMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Задать вопрос", GlobalData.QUESTION),
                InlineKeyboardButton.WithCallbackData("Консультация специалиста", GlobalData.SPECIALIST),
                InlineKeyboardButton.WithCallbackData("Я все узнал, спасибо", GlobalData.DONE),
            }
        });
        await _botClient.SafeSendTextMessageAsync(chatId, "Хотите задать еще вопрос?",replyMarkup:replyMarkup);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }

    public async Task HandleNullAnswer(long chatId)
    {
        var replyMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Задать вопрос", GlobalData.QUESTION),
                InlineKeyboardButton.WithCallbackData("Консультация специалиста", GlobalData.SPECIALIST)
            }
        });
        await _botClient.SafeSendTextMessageAsync(chatId,"Пожалуйста, задайте Ваш вопрос снова.",replyMarkup:replyMarkup);
    }
}