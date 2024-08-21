using MassTransit;
using MassTransitTest.MassTransit.MySaga;
using MassTransitTest.MassTransit.MySaga.Requests;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddMassTransit( options =>
{
    string _serviceBusConnectionString = "Your ServiceBus Connection String Here";
    string _redisConnectionString = "Your Redis Connection String Here";
    // this controls the naming in the service bus
    options.SetKebabCaseEndpointNameFormatter();
    options.AddServiceBusMessageScheduler();

    options.AddSagaStateMachine<MyStateMachine, MyState>(typeof(MyStateDefinition))
                .RedisRepository(r =>
                {
                    r.DatabaseConfiguration(_redisConnectionString);

                    r.KeyPrefix = nameof(MyStateMachine);

                    // Optional, the default is 30 seconds
                    r.LockTimeout = TimeSpan.FromSeconds(90);
                });

    options.UsingAzureServiceBus((context, cfg) =>
    {
        cfg.Host(_serviceBusConnectionString);
        cfg.UseServiceBusMessageScheduler();

        cfg.Send<StateMachineInitRequest>(opts =>
            opts.UseSessionIdFormatter(context => context.Message.CorrelationId.Equals(Guid.Empty) ? "my-api" : context.Message.CorrelationId.ToString()));

        // required to pick up the consumers and auto-detected endpoints for publishers
        cfg.ConfigureEndpoints(context);
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
