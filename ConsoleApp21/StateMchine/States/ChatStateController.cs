using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using static ConsoleApp21.GlobalData;

namespace ConsoleApp21.StateMchine.States;

public class ChatStateController
{
    private readonly ChatStateMachine _chatStateMachine;
    public ChatStateController(ChatStateMachine chatStateMachine)
    {
        _chatStateMachine = chatStateMachine;
    }

    public async Task HandleUpdate(Update update)
    {
        Message message;
        string? data;
        
        switch (update.Type)
        {
            case UpdateType.Message:
                message = update.Message;
                data = update.Message.Text;
                break;
            case UpdateType.CallbackQuery:
                message = update.CallbackQuery.Message;
                data = update.CallbackQuery.Data;
                break;
            default:return;
        }
        var chatId = message.Chat.Id;
        switch (data)
        {
            case START:
                await _chatStateMachine.TransitTo<StartState>(chatId);
                break;
            case CHECK_SUBSCRIPTION:
                await _chatStateMachine.TransitTo<StartState>(chatId);
                break;
            case QUESTION:
                await _chatStateMachine.TransitTo<QuestionState>(chatId);
                break;
            case SPECIALIST:
                await _chatStateMachine.TransitTo<WaitingForNameState>(chatId);
                break;
            case DONE:
                await _chatStateMachine.TransitTo<DoneState>(chatId);
                break;
            default:
                await _chatStateMachine.GetState(chatId).HandleMessage(message);
                break;
        }

        
    }
}