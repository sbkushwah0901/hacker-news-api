using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;


namespace HackerNewsApp.Infrastructure.Tests
{
    public class ExceptionHandlingMiddlewareTests
    {
        [Fact]
        public async Task InvokeAsync_ShouldHandleExceptionAndReturnErrorResponse()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
            var middleware = new ExceptionHandlingMiddleware(next: (innerHttpContext) =>
            {
                throw new Exception("Test exception");
            }, loggerMock.Object);

            var context = new DefaultHttpContext();
            var response = context.Response;
            response.Body = new System.IO.MemoryStream();

            // Act
            await middleware.InvokeAsync(context);

            // Assert
            response.Body.Seek(0, System.IO.SeekOrigin.Begin);
            var reader = new System.IO.StreamReader(response.Body);
            var responseBody = await reader.ReadToEndAsync();
            Assert.Contains("Internal Server Error from the custom middleware.", responseBody);
            Assert.Equal((int)HttpStatusCode.InternalServerError, response.StatusCode);
        }
    }
}
