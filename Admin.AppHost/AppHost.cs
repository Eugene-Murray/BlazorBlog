var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Admin>("admin");

builder.Build().Run();
