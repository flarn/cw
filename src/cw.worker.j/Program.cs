using cw.worker.j;
using CW.Infra.ServiceBus;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();
builder.AddServicebusClient().WithServicebusConsumer();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
