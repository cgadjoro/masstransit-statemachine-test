using MassTransit;

namespace MassTransitTest.MassTransit.MySaga
{
    public class MyState : SagaStateMachineInstance, ISagaVersion
    {
        public Guid CorrelationId { get; set; }

        public string CurrentState { get; set; }

        public int Version { get; set; }
    }
}
