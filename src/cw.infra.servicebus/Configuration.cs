using CW.Core;
using CW.Core.interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace CW.Infra.ServiceBus;

public static class Configuration
{
    public static IHostApplicationBuilder AddServicebusClient(this IHostApplicationBuilder builder)
    {
        builder.AddAzureServiceBusClient("bus");
        builder.Services.AddSingleton<IBusSender, ServiceBusSender>();
        return builder;
    }

    public static IHostApplicationBuilder WithServicebusSender(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IBusSender, ServiceBusSender>();
        return builder;
    }

    public static IHostApplicationBuilder WithServicebusConsumer(this IHostApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IBusConsumer, ServicebusConsumer>();

        builder.Services.Configure<SubscriberConfiguration>(builder.Configuration.GetSection(nameof(SubscriberConfiguration)));
        return builder;
    }
}
