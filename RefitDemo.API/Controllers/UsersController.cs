namespace RefitDemo.API.Controllers
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc;

    using RefitDemo.API.Models;

    /// <summary>
    /// The values controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        /// <summary>
        /// The user service.
        /// </summary>
        private readonly IUserService userService;

        /// <summary>
        /// The client factory.
        /// </summary>
        private readonly IHttpClientFactory clientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController"/> class.
        /// </summary>
        /// <param name="userService">
        /// The user Service.
        /// </param>
        /// <param name="clientFactory">
        /// The client Factory.
        /// </param>
        public UsersController(IUserService userService, IHttpClientFactory clientFactory)
        {
            this.userService = userService;
            this.clientFactory = clientFactory;
        }

        /// <summary>
        /// The get method.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> Get(int page = 1)
        {
            /* var randomUserApi = RestService.For<IUserService>("https://randomuser.me");
            var users = randomUserApi.GetAsync(1); */

            // Call using Refit typed client
            /* var users = await this.userService.GetAsync(page);
            return this.Ok(users); */

            // Call using http named client
            var users = await this.GetUserUsingHttpClient();

            if (users is null)
            {
                return this.NotFound();
            }

            return this.Ok(users);
        }

        /// <summary>
        /// The call GC - Temporary endpoint - Just for testing.
        /// </summary>
        /// <param name="runGc">
        /// The run GC.
        /// </param>
        /// <returns>
        /// The <see cref="IActionResult"/>.
        /// </returns>
        [HttpGet("diag")]
        public IActionResult CallGC([FromQuery] bool runGc = false)
        {
            var beforeGcTotal = GC.GetTotalMemory(false);

            var p = Process.GetCurrentProcess();

            var beforeGcWorkingSet64 = p.WorkingSet64;
            var beforeGcPrivate = p.PrivateMemorySize64;

            if (runGc)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                var afterGcTotal = GC.GetTotalMemory(true);

                var afterGcWorkingSet64 = p.WorkingSet64;
                var afterGcPrivate = p.PrivateMemorySize64;

                var diagnostics = new
                {
                    BeforeGcTotal = beforeGcTotal,
                    AfterGcTotal = afterGcTotal,
                    DifferenceTotal = beforeGcTotal - afterGcTotal,
                    BeforeGcWorkingSet64 = beforeGcTotal,
                    BeforeGcPrivate = beforeGcPrivate,
                    AfterGcWorkingSet64 = afterGcWorkingSet64,
                    AfterGcPrivate = afterGcPrivate
                };

                return this.Ok(diagnostics);

            }
            else
            {
                var diagnostics = new
                {
                    TotalMemory = beforeGcTotal / 1000000,
                    WorkingSet64 = beforeGcWorkingSet64 / 1000000,
                    Private = beforeGcPrivate / 1000000
                };

                return this.Ok(diagnostics);
            }
        }

        /// <summary>
        /// The get user.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        private async Task<Root> GetUserUsingHttpClient(int page = 1)
        {
            Root users = null;
            var client = this.clientFactory.CreateClient("userApi");
            var response = await client.GetAsync("/api/users?page=1");

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();
                users = await JsonSerializer.DeserializeAsync<Root>(
                                stream,
                                new JsonSerializerOptions
                                    {
                                        PropertyNameCaseInsensitive = true,
                                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                        WriteIndented = true,
                                    });
            }

            return users;
        }
    }
}
