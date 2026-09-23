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
        var options = new Mock<IOptionsMonitor<AuthenticationSchemeOptions>>();

        options.Setup(x => x.Get(It.IsAny<string>())).Returns(new AuthenticationSchemeOptions());

        var handler = new ApiKeyAuthenticationHandler(
            options.Object,
            NullLoggerFactory.Instance,
            UrlEncoder.Default);

        var context = new DefaultHttpContext();

        context.Request.Headers["X-API-Key"] = "my-secret-key";

        await handler.InitializeAsync(new AuthenticationScheme("ApiKey", "ApiKey", typeof(ApiKeyAuthenticationHandler)), 
            context);

        var result = await handler.AuthenticateAsync();

        Assert.True(result.Succeeded);
    }
}