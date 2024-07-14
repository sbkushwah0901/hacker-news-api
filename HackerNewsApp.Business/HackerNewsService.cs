using HackerNewsApp.Business.Interface;
using HackerNewsApp.BusinessEntities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace HackerNewsApp.Business
{
    public class HackerNewsService : IHackerNewsService
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private const string NewestStoriesCacheKey = "NewestStories";
        private readonly IConfiguration _configuration;

        public HackerNewsService(HttpClient httpClient, IMemoryCache cache, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _cache = cache;
            _configuration = configuration;
        }

        public async Task<List<HackerNewsStoryModel>> GetNewestStoriesAsync()
        {
            if (_cache.TryGetValue(_configuration.GetSection("NewestStoriesCacheKey").Value, out List<HackerNewsStoryModel> stories))
            {
                return stories;
            }
            var response = await _httpClient.GetAsync("https://hacker-news.firebaseio.com/v0/newstories.json");
            var storyIds = await response.Content.ReadAsAsync<List<int>>();

            stories = new List<HackerNewsStoryModel>();
            foreach (var id in storyIds.Take(Convert.ToInt32(_configuration.GetSection("StoriesCount").Value)))
            {
                var storyResponse = await _httpClient.GetAsync($"https://hacker-news.firebaseio.com/v0/item/{id}.json");
                var story = await storyResponse.Content.ReadAsAsync<HackerNewsStoryModel>();
                stories.Add(story);
            }

            _cache.Set(NewestStoriesCacheKey, stories, TimeSpan.FromMinutes(Convert.ToInt32(_configuration.GetSection("CacheExpiryTimeInMinutes").Value)));

            return stories;
        }

    }
}
