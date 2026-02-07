var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BlazorExperiments>("blazorexperiments");

builder.AddProject<Projects.EM_CMS_GrpcService>("em-cms-grpcservice");

builder.Build().Run();
