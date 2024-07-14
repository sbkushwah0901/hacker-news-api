using HackerNewsApp.Api.Controllers;
using HackerNewsApp.Business.Interface;
using HackerNewsApp.BusinessEntities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace HackerNewsApp.Api.Tests
{
    public class HackerNewsControllerTests
    {
        private readonly Mock<IHackerNewsService> _hackerNewsServiceMock;
        private readonly HackerNewsController _controller;

        public HackerNewsControllerTests()
        {
            _hackerNewsServiceMock = new Mock<IHackerNewsService>();
            _controller = new HackerNewsController(_hackerNewsServiceMock.Object);
        }

        [Fact]
        public async Task GetNewestStories_ShouldReturnOkResult_WithStories()
        {
            // Arrange
            var stories = new List<HackerNewsStoryModel>
            {
                new HackerNewsStoryModel { Id = 1, Title = "Story 1", Url = "http://example.com/1" },
                new HackerNewsStoryModel { Id = 2, Title = "Story 2", Url = "http://example.com/2" }
            };
            _hackerNewsServiceMock.Setup(s => s.GetNewestStoriesAsync()).ReturnsAsync(stories);

            // Act
            var result = await _controller.GetNewestStories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnStories = Assert.IsType<List<HackerNewsStoryModel>>(okResult.Value);
            Assert.Equal(2, returnStories.Count);
            Assert.Equal("Story 1", returnStories[0].Title);
            Assert.Equal("Story 2", returnStories[1].Title);
        }

        [Fact]
        public async Task GetNewestStories_ShouldReturnOkResult_WithoutStories()
        {
            // Arrange
            var stories = new List<HackerNewsStoryModel>();
            _hackerNewsServiceMock.Setup(s => s.GetNewestStoriesAsync()).ReturnsAsync(stories);

            // Act
            var result = await _controller.GetNewestStories();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnStories = Assert.IsType<List<HackerNewsStoryModel>>(okResult.Value);
            Assert.Empty(returnStories);
        }

    }
}
