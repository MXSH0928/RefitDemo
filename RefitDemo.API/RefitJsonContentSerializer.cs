namespace DemoApiFive
{
    using System;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;

    using Refit;

    /// <summary>
    /// SOURCE: https://marcominerva.wordpress.com/2019/11/13/json-serialization-and-deserialization-using-system-text-json-with-refit/
    /// </summary>
    public class RefitJsonContentSerializer : IContentSerializer
    {
        /// <summary>
        /// The serializer options.
        /// </summary>
        private readonly JsonSerializerOptions serializerOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="RefitJsonContentSerializer"/> class.
        /// </summary>
        /// <param name="serializerOptions">
        /// The serializer options.
        /// </param>
        public RefitJsonContentSerializer(JsonSerializerOptions serializerOptions = null)
        {
            this.serializerOptions = serializerOptions ?? new JsonSerializerOptions
                                                              {
                                                                  PropertyNameCaseInsensitive = true,
                                                                  PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                                                                  WriteIndented = true,
                                                              };
        }

        public async Task<T> DeserializeAsync<T>(HttpContent content)
        {
            using Stream utf8Json = await content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<T>(utf8Json, this.serializerOptions);
        }

        public Task<HttpContent> SerializeAsync<T>(T item)
        {
            var json = JsonSerializer.Serialize(item, serializerOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            return Task.FromResult((HttpContent)content);
        }
    }
}
