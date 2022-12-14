using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text.Json;
using GroupPostsPollingService.Models;
using GroupPostsPollingService.Utilities;
using RestSharp;

namespace GroupPostsPollingService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IFbGroupPostsClient _fbGroupPostsClient;

    public Worker(ILogger<Worker> logger, IFbGroupPostsClient fbGroupPostsClient )
    {
        _logger = logger;
        _fbGroupPostsClient = fbGroupPostsClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //oauth/access_token?grant_type=fb_exchange_token&client_id=786802162382083&client_secret=15bde87a5414252eeafa455c072c816d&fb_exchange_token=EAALLl6dltQMBAMmcumnCaZAxYpRdiBhZA2wbn7d3slJj7uxlgY2cUjawwf7Srio5jCc2embWIzGjibKfVTg6iahFsFMhggcTZCXZADMQhHv5gZAbreT8trOqKZBHiYiRZAfWNe2gehr9LLIIRZCdwKGB8jM9035y10uiwiZCY16haZCTKJFcyWd9y3vAZC4wUrQsyY6MlOTvHo2V87F8Ma0PbGZB8evREwYbaO3JQYNWlzkpN84PrkeLyUa3 
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            var accessToken = "EAALLl6dltQMBAJgnZCATfWMhEQWrc5ABwuM1rz3FEmP2P0qEYAduruMQN6PafNRDsgm7dIAZBpwMFvRV5SMY8MVyaO7zj2BZBbYzI3NMvbH2eESlMndMkEZAnZAWAfmC91FUKUPIebZAw0vo61qtEt5AkkPglIj65SeaZBOlDOX7gZDZD";
            var client = new RestClient(string.Format("{0}{1}", "https://graph.facebook.com/1287074015193292/feed?access_token=", accessToken));

            var request = new RestRequest();
            var response = client.Execute(request);

            JsonSerializerOptions options = new JsonSerializerOptions();
            options.Converters.Add(new DateTimeOffsetConverterUsingDateTimeParse());
            var listofPosts = JsonSerializer.Deserialize<FacebookGroupPostsDataModel>(json: response.Content, options).data.ToList();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var tags = new List<string>();
                tags.Add("İSTANBUL");
 
            var lastRecord = await _fbGroupPostsClient.GetAllAsync("1", "UpdatedDate", "Desc", tags, null, 1);

            if (listofPosts[0].id != lastRecord.FirstOrDefault()?.Id)
            {
                var index = 0;
                while (listofPosts[index].id != lastRecord.FirstOrDefault()?.Id)
                {
                    if (listofPosts[index].message != null) listofPosts[index].tags = GetMatchingTags(listofPosts[index].message);
                    index++;
                }


                listofPosts.RemoveRange(index, (listofPosts.Count) - index);
                var createGroupPostDtos = listofPosts.Select(x => new CreateGroupPostDto
                {
                    Id = x.id,
                    Message = x.message,
                    UpdatedDate = DateTimeOffset.Parse(x.updated_time),
                    Tags = x.tags.ToList(),
                    GroupId = x.id.Split('_')[0]
                });
                await _fbGroupPostsClient.FbGroupPostAsync(createGroupPostDtos);
            }
            watch.Stop();
            await Task.Delay(60 * 1000, stoppingToken);
        }
    }

    public IEnumerable<string> GetMatchingTags(string message)
    {
        
        var illerArray = new string[] {"Adana", "Adıyaman", "Afyon", "Ağrı", "Amasya", "Ankara", "Antalya", "Artvin",
        "Aydın", "Balıkesir", "Bilecik", "Bingöl", "Bitlis", "Bolu", "Burdur", "Bursa", "Çanakkale",
        "Çankırı", "Çorum", "Denizli", "Diyarbakır", "Edirne", "Elazığ", "Erzincan", "Erzurum", "Eskişehir",
        "Gaziantep", "Giresun", "Gümüşhane", "Hakkari", "Hatay", "Isparta", "Mersin", "İstanbul", "İzmir",
        "Kars", "Kastamonu", "Kayseri", "Kırklareli", "Kırşehir", "Kocaeli", "Konya", "Kütahya", "Malatya",
        "Manisa", "Kahramanmaraş", "Mardin", "Muğla", "Muş", "Nevşehir", "Niğde", "Ordu", "Rize", "Sakarya",
        "Samsun", "Siirt", "Sinop", "Sivas", "Tekirdağ", "Tokat", "Trabzon", "Tunceli", "Şanlıurfa", "Uşak",
        "Van", "Yozgat", "Zonguldak", "Aksaray", "Bayburt", "Karaman", "Kırıkkale", "Batman", "Şırnak",
        "Bartın", "Ardahan", "Iğdır", "Yalova", "Karabük", "Kilis", "Osmaniye", "Düzce" };
        var ililceler = GetIlceler().ToList();
        var locationsList = illerArray.Select(x => new string(x.ToUpper())).ToList();

        var blacklist = new HashSet<string>();
        blacklist.Add("ARAÇ");

        var tags = new HashSet<string>();
        var wordsArray = message.ToUpper().Split(' ');
        for(var i = 0; i< wordsArray.Length; i++)
        {
            for(var j=0; j< locationsList.Count; j++)
            {
                if (wordsArray[i].Contains(locationsList[j]))
                    if (!tags.Contains(locationsList[j])) tags.Add(locationsList[j]);
            }
        }

        for (var i = 0; i < wordsArray.Length; i++)
        {
            for (var j = 0; j < ililceler.Count; j++)
            {
                if (wordsArray[i].Contains(ililceler[j].ilce.ToUpper()) && !blacklist.Contains(ililceler[j].ilce.ToUpper()) && ililceler[j].ilce.Length > 3)
                {
                    if (!tags.Contains(ililceler[j].ilce.ToUpper())) tags.Add(ililceler[j].ilce.ToUpper());
                    if (!tags.Contains(ililceler[j].il.ToUpper())) tags.Add(ililceler[j].il.ToUpper());
                }
            }
        }

        return tags;
    }

    public char RemapInternationalCharToAscii(char c)
    {
        string s = c.ToString().ToUpperInvariant();
        if ("àåáâäãåą".Contains(s))
        {
            return 'a';
        }
        else if ("èéêëę".Contains(s))
        {
            return 'e';
        }
        else if ("ìíîïı".Contains(s))
        {
            return 'i';
        }
        else if ("òóôõöøőð".Contains(s))
        {
            return 'o';
        }
        else if ("ùúûüŭů".Contains(s))
        {
            return 'u';
        }
        else if ("çćčĉ".Contains(s))
        {
            return 'c';
        }
        
        else if ("śşšŝ".Contains(s))
        {
            return 's';
        }
        
        else if ("ğĝ".Contains(s))
        {
            return 'g';
        }
        
        else
        {
            return c;
        }
    }

    public IEnumerable<ililceModel> GetIlceler()
    {
        string currentDirectory = Directory.GetParent(Environment.CurrentDirectory).FullName;
        var filePath = Path.Combine(currentDirectory, "il-ilceJson.txt");
        var ilcelerr = File.ReadAllText(@"C:\Users\babay\source\repos\Nakliyeciler-GroupPostsPollingService\GroupPostsPollingService\il-ilceJson.txt");
        var ilceler = File.ReadAllText(filePath);
        var data = JsonSerializer.Deserialize<IEnumerable<ililceModel>>(json: ilceler);
        return data;
    }

}

