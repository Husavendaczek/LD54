using SystemBase.Core;
using SystemBase.Core.StateMachineBase;
using SystemBase.GameState.Messages;
using UniRx;

namespace SystemBase.GameState.States
{
    [NextValidStates(typeof(Running), typeof(GameOver))]
    public class Paused : BaseState<Game>
    {
        public override void Enter(StateContext<Game> context)
        {
            MessageBroker.Default.Receive<GameMsgUnpause>()
                .Subscribe(unpause => context.GoToState(new Running()))
                .AddTo(this);
            
            MessageBroker.Default.Receive<GameMsgEnd>()
                .Subscribe(end => context.GoToState(new GameOver()))
                .AddTo(this);
        }
    }
}
