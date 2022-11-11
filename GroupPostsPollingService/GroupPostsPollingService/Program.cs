using GroupPostsPollingService;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddSingleton<IFbGroupPostsClient>(new FbGroupPostsClient("https://localhost:7141/"));
    })
    .Build();

await host.RunAsync();

