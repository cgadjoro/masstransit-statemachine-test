using MassTransit;
using MassTransitTest.MassTransit.MySaga.Requests;
using MassTransitTest.MassTransitComponents.MySaga.Requests;
namespace MassTransitTest.MassTransit.MySaga
{
    public class MyStateMachine : MassTransitStateMachine<MyState>
    {
        private readonly ILogger<MyStateMachine> _logger;
        public State InProgress { get; }

        public Event<StateMachineInitRequest> StateMachineInitRequestEvent { get; }

        public Event<DoWorkRequest> DoWorkRequestEvent { get; }

        public MyStateMachine(ILogger<MyStateMachine> logger) 
        {
            _logger = logger;
            InstanceState(x => x.CurrentState);
            Event(() => StateMachineInitRequestEvent, x => x.CorrelateById(context => context.Message.CorrelationId));
            Event(() => DoWorkRequestEvent, x => x.CorrelateById(context => context.Message.CorrelationId));

            Initially(
                When(StateMachineInitRequestEvent)
                .Then(x =>
                {
                    _logger.LogInformation(
                         "User Invitation State Machine started with CorrelationId: '{CorrelationId}'", x.Message.CorrelationId);
                })
                 .PublishAsync(context =>
                 {
                     DoWorkRequest request = new()
                     {
                         CorrelationId = context.Message.CorrelationId
                     };
                     return context.Init<DoWorkRequest>(request);
                 })
                .TransitionTo(InProgress),
                Ignore(DoWorkRequestEvent));

            During(
                InProgress,
                When(DoWorkRequestEvent)
                .Then(x =>
                {
                    Task.Delay(2000).Wait();
                    _logger.LogInformation("We are done here");
                })
                .Finalize(),
                Ignore(StateMachineInitRequestEvent));

            SetCompletedWhenFinalized();
        }
    }
}
