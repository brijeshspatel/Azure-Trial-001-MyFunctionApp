using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MyFunctionApp;

namespace MyFunctionApp.Tests
{
    public class StartupTests
    {
        [Fact]
        public void Configure_Registers_SerilogLoggerProvider()
        {
            // Arrange
            var services = new ServiceCollection();
            var builderMock = new Mock<Microsoft.Azure.Functions.Extensions.DependencyInjection.IFunctionsHostBuilder>();
            builderMock.Setup(b => b.Services).Returns(services);
            var startup = new Startup();

            // Act
            startup.Configure(builderMock.Object);

            // Assert
            var provider = services.FirstOrDefault(s => s.ServiceType == typeof(ILoggerProvider));
            Assert.NotNull(provider);
            Assert.Equal(typeof(Serilog.Extensions.Logging.SerilogLoggerProvider), provider.ImplementationInstance?.GetType());
        }
    }
}
