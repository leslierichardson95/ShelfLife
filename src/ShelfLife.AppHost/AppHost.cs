var builder = DistributedApplication.CreateBuilder(args);

var api = builder.AddProject<Projects.ShelfLife_Api>("api");

builder.AddProject<Projects.ShelfLife_Web>("web")
    .WithExternalHttpEndpoints()
    .WithReference(api)
    .WaitFor(api);

builder.Build().Run();
