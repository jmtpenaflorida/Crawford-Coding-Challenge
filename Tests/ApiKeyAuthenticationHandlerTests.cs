using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using Moq;

public class ApiKeyAuthenticationHandlerTests
{
    [Fact]
    public async Task ValidApiKey_ReturnsSuccess()
    {
        // Arrange
        var handler = await ConfigureHandler("my-secret-key");

        // Act
        var result = await handler.AuthenticateAsync();

        // Assert
        Assert.True(result.Succeeded);
    }

    [Fact]
    public async Task ValidApiKey_ReturnsFailed()
    {
        // Arrange
        var handler = await ConfigureHandler("my-secret-key-wrong");

        // Act
        var result = await handler.AuthenticateAsync();
        
        // Assert
        Assert.False(result.Succeeded);
        Assert.Equal("Invalid API key", result.Failure!.Message);
    }

    private async Task<ApiKeyAuthenticationHandler> ConfigureHandler(string apiKey)
    {
        var options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();

        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());

        var handler = new ApiKeyAuthenticationHandler(
            options.Object,
            NullLoggerFactory.Instance,
            UrlEncoder.Default);

        var context = new DefaultHttpContext();

        context.Request.Headers["X-API-Key"] = apiKey;

        await handler.InitializeAsync(new AuthenticationScheme("ApiKey", "ApiKey", typeof(ApiKeyAuthenticationHandler)), 
            context);

        return handler;
    } 
}