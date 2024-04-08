using System.Collections.Concurrent;
using ConsoleApp21.StateMchine.States;
using Telegram.Bot;

namespace ConsoleApp21.StateMchine;

public class ChatStateMachine
{
    private readonly ConcurrentDictionary<long, ChatStateBase> _chatStates = new();
    private readonly Dictionary<Type, Func<ChatStateBase>> _states = new();

    public ChatStateMachine(ITelegramBotClient botClient,AppSettings settings,IChatGptService chatGptService)
    {
        _states[typeof(IdleState)] = () => new IdleState(this);
        _states[typeof(StartState)] = () => new StartState(this,botClient,settings.AgencyName,chatGptService);
       
    }

    public ChatStateBase GetState(long chatId)
    {
        return !_chatStates.TryGetValue(chatId, out var state) ? _states[typeof(IdleState)].Invoke():state;
    }

    public async Task TransitTo<T>(long chatId) where T : ChatStateBase
    {
        if (_chatStates.TryGetValue(chatId, out var currentState))
        {
            await currentState.OnExit(chatId);
        }

        var newState = _states[typeof(T)]();
        _chatStates[chatId] = newState;
        await newState.OnEnter(chatId);

    }
}