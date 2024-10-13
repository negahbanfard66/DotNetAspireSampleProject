var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("outputcache");
var api = builder.AddProject<Projects.Api>("nwsapi")
    .WithReference(cache);

builder.AddProject<Projects.MyWeatherHub>("www")
    .WithReference(api);

builder.Build().Run();
