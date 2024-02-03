using System.ServiceModel.Syndication;
using System.Xml;
using ContentCollection.Models;
using Microsoft.Extensions.DependencyInjection;

namespace ContentCollection.Services;

public class ScheduledService(IServiceProvider serviceProvider) : IDisposable
{
    public static event Action<string>? OnNewAnimeUpdate;
    private Timer? _timer;

    private readonly List<string> _names =
        ["Frieren subsplease", "Kusuriya no subsplease", "Boku no kokoro yabai subsplease", "Ember shaman king flowers"];

    private bool _initialLoad = true;

    public void Setup()
    {
        _timer = new Timer(FetchRssChanges, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    }

    private void FetchRssChanges(object? state)
    {
        Task.Run(() =>
        {
            using var scope = serviceProvider.CreateScope();
            var cache = scope.ServiceProvider.GetRequiredService<CacheService>();

            Parallel.ForEach(_names, animeName =>
            {
                Console.WriteLine($"[{DateTimeOffset.UtcNow:HH:mm:ss.fff} - SCHEDULED] STARTING - {animeName}");
                var reader = XmlReader.Create($"https://nyaa.si/?page=rss&q={animeName}%201080&c=0_0&f=0&m");
                var feed = SyndicationFeed.Load(reader);
                reader.Close();

                var existingEpisodes = cache.Episodes.Where(p => p.AnimeName == animeName).ToList();

                foreach (var item in feed.Items)
                {
                    if (existingEpisodes.Any(p => p.Title == item.Title.Text)) continue;

                    var newEpisode = new EpisodeInfo
                    {
                        AnimeName = animeName,
                        Title = item.Title.Text,
                        Magnet = item.Links.First().Uri.ToString()
                    };
                    cache.Episodes.Add(newEpisode);
                }

                if (_initialLoad) OnNewAnimeUpdate?.Invoke(animeName);

                Console.WriteLine($"[{DateTimeOffset.UtcNow:HH:mm:ss.fff} - SCHEDULED] COMPLETED - {animeName}");
            });
            _initialLoad = false;
        });
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
