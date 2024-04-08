using Telegram.Bot;

namespace ConsoleApp21.StateMchine.States;

public class QuestionState:ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    private readonly IChatGptService _chatGptService;

    public QuestionState(,ITelegramBotClient botClient, IChatGptService chatGptService)
    {
        _botClient = botClient;
        _chatGptService = chatGptService;
    }
}