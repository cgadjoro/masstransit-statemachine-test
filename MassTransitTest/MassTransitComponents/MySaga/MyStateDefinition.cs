using MassTransit;

namespace MassTransitTest.MassTransit.MySaga
{
    public class MyStateDefinition : SagaDefinition<MyState>
    {
        protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator, ISagaConfigurator<MyState> sagaConfigurator, IRegistrationContext context)
        {
            endpointConfigurator.UseInMemoryInboxOutbox(context);

            if (endpointConfigurator is IServiceBusReceiveEndpointConfigurator sbc)
            {
                sbc.RequiresSession = true;
            }
        }
    }
}
