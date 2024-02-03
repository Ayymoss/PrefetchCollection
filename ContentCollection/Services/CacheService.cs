using System.Collections.Concurrent;
using ContentCollection.Models;

namespace ContentCollection.Services;

public class CacheService
{
    public ConcurrentBag<EpisodeInfo> Episodes { get; set; } = [];
}
