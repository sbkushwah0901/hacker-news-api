using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using HackerNewsApp.BusinessEntities;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace HackerNewsApp.Business.Tests
{
    public class HackerNewsServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _memoryCache;
        private readonly HackerNewsService _hackerNewsService;
        private readonly IConfiguration _configuration;

        public HackerNewsServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object);
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            { {"NewestStoriesCacheKey", "NewestStories"},{"CacheExpiryTimeInMinutes", "5"}, {"StoriesCount", "200"} }).Build();
            _hackerNewsService = new HackerNewsService(_httpClient, _memoryCache, _configuration);
        }

        [Fact]
        public async Task GetNewestStoriesAsync_ShouldReturnStories_WhenStoriesAreCached()
        {
            // Arrange
            var stories = new List<HackerNewsStoryModel>
            {
                new HackerNewsStoryModel { Id = 1, Title = "Story 1", Url = "http://example.com/1" },
                new HackerNewsStoryModel { Id = 2, Title = "Story 2", Url = "http://example.com/2" }
            };
            _memoryCache.Set("NewestStories", stories);

            // Act
            var result = await _hackerNewsService.GetNewestStoriesAsync();

            // Assert
            Assert.Equal(2, result.Count);
            Assert.Equal("Story 1", result[0].Title);
            Assert.Equal("Story 2", result[1].Title);
        }
    }
}
