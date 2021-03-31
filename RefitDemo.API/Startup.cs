namespace RefitDemo.API
{
    using System;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
    
    using System.Net.Http;
    using Polly;
    using Polly.Extensions.Http;

    using Refit;

    /// <summary>
    /// The startup.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="configuration">
        /// The configuration.
        /// </param>
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        /// <summary>
        /// Gets the configuration.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// The configure.
        /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// </summary>
        /// <param name="app">
        /// The app.
        /// </param>
        /// <param name="env">
        /// The env.
        /// </param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "DemoApiFive v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        /// <summary>
        /// The configure services.
        /// This method gets called by the runtime. Use this method to add services to the container.
        /// </summary>
        /// <param name="services">
        /// The services.
        /// </param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(
                c => { c.SwaggerDoc("v1", new OpenApiInfo { Title = "DemoApiFive", Version = "v1" }); });

            // Refit help build "Typed HttpClient" easily
            services.AddRefitClient<IUserService>(new RefitSettings()
            {
                // To use "System.Text.Json"
                // ContentSerializer = new RefitJsonContentSerializer()
            }).ConfigureHttpClient(
                c =>
                    {
                        c.BaseAddress = new Uri("https://reqres.in");
                    });

            // Register named http client
            services.AddHttpClient(
                "userApi",
                c =>
                    {
                        c.BaseAddress = new Uri("https://reqres.in");
                })
                // Set lifetime to five minutes
                .SetHandlerLifetime(TimeSpan.FromMinutes(5))
                // The AddPolicyHandler() method is what adds policies to the HttpClient objects
                .AddPolicyHandler(GetRetryPolicy());

            /**********************************************************************************
             * Polly is a transient-fault-handling library that helps developers add resiliency to their
             * applications, by using some pre-defined policies in a fluent and thread-safe manner.
             **********************************************************************************/
        }

        /// <summary>
        /// Http Retry Policy can be defined in a separate method within the Startup.cs file
        /// </summary>
        /// <returns></returns>
        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            /**************************************************************************************************************
             * With Polly, you can define a Retry policy with the number of retries, the exponential backoff configuration,
             * and the actions to take when there's an HTTP exception, such as logging the error. In this case, the policy
             * is configured to try six times with an exponential retry, starting at two seconds.
             *
             * More on retry pattern:
             * https://docs.microsoft.com/en-us/azure/architecture/patterns/retry
             * https://docs.microsoft.com/en-us/dotnet/architecture/microservices/implement-resilient-applications/implement-http-call-retries-exponential-backoff-polly
             **************************************************************************************************************/

            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
        }
    }
}