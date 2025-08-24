var builder = DistributedApplication.CreateBuilder(args);

var servicebus = builder.AddAzureServiceBus("bus").RunAsEmulator();
var topic = servicebus.AddServiceBusTopic("orders");
topic.AddServiceBusSubscription("ext-json");
topic.AddServiceBusSubscription("ext-csv");


var api = builder.AddProject<Projects.cw_api>("cw-api").WithReference(servicebus).WaitFor(servicebus);
var csvWorker = builder.AddProject<Projects.cw_worker_c>("cw-worker-c").WithReference(servicebus).WaitFor(servicebus);
var jsonWorker = builder.AddProject<Projects.cw_worker_j>("cw-worker-j").WithReference(servicebus).WaitFor(servicebus);


builder.Build().Run();
