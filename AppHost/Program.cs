var builder = DistributedApplication.CreateBuilder(args);
builder.AddProject<Projects.Api>("nws");
builder.AddProject<Projects.MyWeatherHub>("www");
builder.Build().Run();
