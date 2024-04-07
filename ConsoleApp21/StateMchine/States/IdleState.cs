using Telegram.Bot.Types;

namespace ConsoleApp21.StateMchine.States;

public class IdleState:ChatStateBase
{
    private ChatStateBase _chatStateBaseImplementation;

    public IdleState(ChatStateMachine stateMachine) : base(stateMachine)
    {
        
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }
}