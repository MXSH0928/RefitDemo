namespace DemoApiFive.Models
{
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    /// <summary>
    /// The root.
    /// </summary>
    public class Root
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public List<Data> Data { get; set; }

        /// <summary>
        /// Gets or sets the page.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the per page.
        /// </summary>
        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        /// <summary>
        /// Gets or sets the support.
        /// </summary>
        public Support Support { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// Gets or sets the total pages.
        /// </summary>
        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }
    }
}
