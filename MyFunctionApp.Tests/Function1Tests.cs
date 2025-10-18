using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using AutoFixture.Xunit2;
using MyFunctionApp;

namespace MyFunctionApp.Tests
{
    public class Function1Tests
    {
        [Theory, AutoData]
        public async Task Run_Returns_Personalized_Message_When_Name_In_Query(string name)
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString($"?name={name}");
            var loggerMock = new Mock<ILogger>();

            // Act
            var result = await Function1.Run(context.Request, loggerMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains($"Hello, {name}", okResult.Value?.ToString() ?? string.Empty);
        }

        [Theory, AutoData]
        public async Task Run_Returns_Personalized_Message_When_Name_In_Body(string name)
        {
            // Arrange
            var context = new DefaultHttpContext();
            var json = $"{{\"name\":\"{name}\"}}";
            var bytes = Encoding.UTF8.GetBytes(json);
            context.Request.Body = new MemoryStream(bytes);
            context.Request.ContentLength = bytes.Length;
            var loggerMock = new Mock<ILogger>();

            // Act
            var result = await Function1.Run(context.Request, loggerMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains($"Hello, {name}", okResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task Run_Returns_Default_Message_When_Name_Missing()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("{}"));
            var loggerMock = new Mock<ILogger>();

            // Act
            var result = await Function1.Run(context.Request, loggerMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Pass a name in the query string or in the request body", okResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task Run_Returns_Default_Message_When_Body_Is_Empty()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream();
            var loggerMock = new Mock<ILogger>();

            // Act
            var result = await Function1.Run(context.Request, loggerMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Pass a name in the query string or in the request body", okResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task Run_Returns_Default_Message_When_Body_Is_Null()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Body = Stream.Null;
            var loggerMock = new Mock<ILogger>();

            // Act
            var result = await Function1.Run(context.Request, loggerMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Pass a name in the query string or in the request body", okResult.Value?.ToString() ?? string.Empty);
        }

        [Fact]
        public async Task Run_Returns_Default_Message_When_Body_Is_Invalid_Json()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.Body = new MemoryStream(Encoding.UTF8.GetBytes("not a json"));
            var loggerMock = new Mock<ILogger>();

            // Act
            var result = await Function1.Run(context.Request, loggerMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("Pass a name in the query string or in the request body", okResult.Value?.ToString() ?? string.Empty);
        }

        [Theory, AutoData]
        public async Task Run_Query_Name_Takes_Precedence_Over_Body(string queryName, string bodyName)
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString($"?name={queryName}");
            var json = $"{{\"name\":\"{bodyName}\"}}";
            var bytes = Encoding.UTF8.GetBytes(json);
            context.Request.Body = new MemoryStream(bytes);
            context.Request.ContentLength = bytes.Length;
            var loggerMock = new Mock<ILogger>();

            // Act
            var result = await Function1.Run(context.Request, loggerMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains($"Hello, {queryName}", okResult.Value?.ToString() ?? string.Empty);
        }

        [Theory]
        [InlineData("!@£$%^&*()_+-=|~`[]{};:'\",.<>/?")]
        [InlineData("Alice Bob Charlie")]
        [InlineData("")]
        public async Task Run_Handles_Special_And_Empty_Characters(string name)
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString($"?name={name}");
            var loggerMock = new Mock<ILogger>();

            // Act
            var result = await Function1.Run(context.Request, loggerMock.Object);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            if (string.IsNullOrEmpty(name))
            {
                Assert.Contains("Pass a name in the query string or in the request body", okResult.Value?.ToString() ?? string.Empty);
            }
            else
            {
                Assert.StartsWith($"Hello, {name[..Math.Min(name.Length, 6)]}", okResult.Value?.ToString() ?? string.Empty);
            }
        }

        [Fact]
        public async Task Run_Logs_Information_Message()
        {
            // Arrange
            var context = new DefaultHttpContext();
            context.Request.QueryString = new QueryString("?name=TestUser");
            var loggerMock = new Mock<ILogger>();
            
            // Act
            await Function1.Run(context.Request, loggerMock.Object);
            
            // Assert
            loggerMock.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("C# HTTP trigger function processed a request.")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once);
        }
    }
}
