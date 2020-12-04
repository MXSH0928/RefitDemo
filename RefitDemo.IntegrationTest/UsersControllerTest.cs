namespace RefitDemo.IntegrationTest
{
    using System.Diagnostics;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.TestHost;
    using Microsoft.Extensions.Hosting;

    using RefitDemo.API.Models;

    using Xunit;

    /// <summary>
    /// The users controller test.
    /// </summary>
    public class UsersControllerTest
    {
        /// <summary>
        /// The basic end point test.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [Fact]
        public async Task BasicEndPointTest()
        {
            // ARRANGE
            var hostBuilder = new HostBuilder()
                .ConfigureWebHost(webHost =>
                    {
                        // Add TestServer
                        webHost.UseTestServer();
                        webHost.UseStartup<API.Startup>();
                    });

            var serializerOptions = new JsonSerializerOptions
                                                             {
                                                                 PropertyNameCaseInsensitive = true,
                                                                 PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                                 WriteIndented = true,
                                                             };

            // Create and start up the host
            var host = await hostBuilder.StartAsync();

            // Create an HttpClient which is setup for the test host
            var client = host.GetTestClient();

            // ACT
            var response = await client.GetAsync("/api/users");

            // ASSERT
            using Stream utf8Json = await response.Content.ReadAsStreamAsync();
            var users = await JsonSerializer.DeserializeAsync<Root>(utf8Json, serializerOptions);

            Assert.NotNull(users);
            Assert.NotEmpty(users.Data);
            // Assert.Collection(users.Data, item => Assert.NotEmpty(item.Email));
        }
    }
}
