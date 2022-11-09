using System.Text.Json;
using GroupPostsPollingService.Models;
using RestSharp;

namespace GroupPostsPollingService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            var accessToken = "EAALLl6dltQMBAIngaXlyRrZCtj8QZArQ4JZCCl6ZArPLEULAkfwx1zcHGhkLm6UpYKZBGs8XKzSCAqR9AqZCyWcMRTSZBNnnmSxqVZClp4dEnokX4gt5tmhhYNf8JVG5G1AvzxBuH6hdZBJ28v839kMEtEj8HIxV1pssZAPn270Tj7FAZDZD";
            var client = new RestClient(string.Format("{0}{1}", "https://graph.facebook.com/1287074015193292/feed?access_token=", accessToken));

            var request = new RestRequest();
            var response = client.Execute(request);
            var data = JsonSerializer.Deserialize<FacebookGroupPostsDataModel>(json: response.Content);
            await Task.Delay(60 * 1000, stoppingToken);
        }
    }
}

