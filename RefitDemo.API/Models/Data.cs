namespace DemoApiFive.Models
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// The user.
    /// </summary>
    public class Data
    {
        /// <summary>
        /// Gets or sets the avatar.
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the first name.
        /// </summary>
        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the last name.
        /// </summary>
        [JsonPropertyName("last_name")]
        public string LastName { get; set; }
    }
}
