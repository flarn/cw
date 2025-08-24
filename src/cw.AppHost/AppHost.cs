var builder = DistributedApplication.CreateBuilder(args);

var servicebus = builder.AddAzureServiceBus("bus").RunAsEmulator();
var topic = servicebus.AddServiceBusTopic("orders");
topic.AddServiceBusSubscription("ext-json");
topic.AddServiceBusSubscription("ext-csv");


var api = builder.AddProject<Projects.cw_api>("cw-api").WithReference(servicebus);


builder.Build().Run();
