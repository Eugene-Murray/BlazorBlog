var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Admin>("admin");

builder.AddProject<Projects.EM_CMS_GrpcService>("em-cms-grpcservice");

builder.Build().Run();
