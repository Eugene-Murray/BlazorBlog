var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.EM_CMS_Admin>("em-cms-admin");

builder.Build().Run();
