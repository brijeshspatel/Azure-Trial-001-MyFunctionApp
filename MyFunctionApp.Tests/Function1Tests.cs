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
            Assert.Contains($"Hello, {name}", okResult.Value.ToString());
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
            Assert.Contains($"Hello, {name}", okResult.Value.ToString());
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
            Assert.Contains("Pass a name in the query string or in the request body", okResult.Value.ToString());
        }
    }
}
