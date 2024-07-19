using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using HackerNewsApp.BusinessEntities;
using HackerNewsApp.Repository.Interface;
using Microsoft.Extensions.Configuration;
using HackerNewsApp.Infrastructure;
using Moq.Protected;
using System.Threading;
using System;

namespace HackerNewsApp.Business.Tests
{
    public class HackerNewsServiceTests
    {
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly IMemoryCache _memoryCache;
        private readonly HackerNewsService _hackerNewsService;
        private readonly IConfiguration _configuration;
        private readonly Mock<IHackerNewsRepository> _hackerNewsRepositoryMock;

        public HackerNewsServiceTests()
        {
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _hackerNewsRepositoryMock = new Mock<IHackerNewsRepository>();
            _configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
            { {"NewestStoriesCacheKey", "NewestStories"},{"CacheExpiryTimeInMinutes", "5"}, {"StoriesCount", "200"} }).Build();
            _hackerNewsService = new HackerNewsService(_memoryCache, _configuration, _hackerNewsRepositoryMock.Object);
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

        [Fact]
        public async Task GetNewestStoriesAsync_ShouldThrowCustomException_WhenNoNewStoryIdsFound()
        {
            // Arrange
            var httpClientMock = new Mock<HttpMessageHandler>();
            httpClientMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => req.RequestUri.ToString().Contains("newstories")),
                    ItExpr.IsAny<CancellationToken>())
                .Throws(new Exception("Test exception"));

            // Act & Assert
            await Assert.ThrowsAsync<CustomException>(() => _hackerNewsService.GetNewestStoriesAsync());
        }
    }
}
