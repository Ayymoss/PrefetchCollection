using ContentCollection.Services;

namespace ContentCollection;

public class AppEntry(CacheService cache)
{
    public async Task Run()
    {
        var search = string.Empty;
        ScheduledService.OnNewAnimeUpdate += OnNewAnimeUpdate;

        while (true)
        {
            Console.WriteLine($"[{DateTimeOffset.UtcNow:HH:mm:ss.fff} - USER] Getting episodes...");

            var query = cache.Episodes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var tempSearch = search;
                query = query.Where(x => x.AnimeName.Contains(tempSearch, StringComparison.OrdinalIgnoreCase));
            }

            var episodes = query.ToList();
            foreach (var episode in episodes)
            {
                Console.WriteLine($"[{DateTimeOffset.UtcNow:HH:mm:ss.fff} - USER] Anime: {episode.AnimeName}\n\tTitle: {episode.Title}");
            }

            Console.WriteLine($"[{DateTimeOffset.UtcNow:HH:mm:ss.fff} - USER] Total episodes: {episodes.Count}");
            Console.WriteLine(
                $"[{DateTimeOffset.UtcNow:HH:mm:ss.fff} - USER] Type the anime to search for, or press enter to refresh the list: ");
            search = Console.ReadLine();
            if (search?.Equals("Q", StringComparison.OrdinalIgnoreCase) == true) break;
            Console.WriteLine($"[{DateTimeOffset.UtcNow:HH:mm:ss.fff} - USER] Searching for: {search}");
        }
    }

    private void OnNewAnimeUpdate(string anime)
    {
        // This could trigger a UI refresh or a notification
        Console.WriteLine($"[{DateTimeOffset.UtcNow:HH:mm:ss.fff} - EVENT] New anime episodes: {anime}");
    }
}
