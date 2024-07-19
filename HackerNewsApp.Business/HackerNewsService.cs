using HackerNewsApp.Business.Interface;
using HackerNewsApp.BusinessEntities;
using HackerNewsApp.Infrastructure;
using HackerNewsApp.Repository.Interface;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace HackerNewsApp.Business
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly IMemoryCache _cache;
        private const string NewestStoriesCacheKey = "NewestStories";
        private readonly IConfiguration _configuration;
        private readonly IHackerNewsRepository _hackerNewsRepository;
        

        public HackerNewsService(IMemoryCache cache, IConfiguration configuration, IHackerNewsRepository hackerNewsRepository)
        {
            _cache = cache;
            _configuration = configuration;
            _hackerNewsRepository = hackerNewsRepository;
        }

        public async Task<List<HackerNewsStoryModel>> GetNewestStoriesAsync()
        {
            if (_cache.TryGetValue(_configuration.GetSection("NewestStoriesCacheKey").Value, out List<HackerNewsStoryModel> stories))
            {
                return stories;
            }
            var storyIds = await _hackerNewsRepository.GetNewStoryIdsAsync();
            if (storyIds == null || !storyIds.Any())
            {
                throw new CustomException("No new stories found.", HttpStatusCode.NotFound);
            }
            stories = new List<HackerNewsStoryModel>();
            foreach (var id in storyIds.Take(Convert.ToInt32(_configuration.GetSection("StoriesCount").Value)))
            {
                var storyResponse = await _hackerNewsRepository.GetStoryAsync(id);
                stories.Add(storyResponse);
            }

            _cache.Set(NewestStoriesCacheKey, stories, TimeSpan.FromMinutes(Convert.ToInt32(_configuration.GetSection("CacheExpiryTimeInMinutes").Value)));

            return stories;
        }

    }
}
