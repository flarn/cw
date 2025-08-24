using CW.Core.interfaces;
using CW.Infra.ServiceBus;
using CW.Worker.Shared;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.AddServicebusClient().WithServicebusConsumer();
builder.Services.AddSingleton<IExternalSystem, CsvSystem>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
