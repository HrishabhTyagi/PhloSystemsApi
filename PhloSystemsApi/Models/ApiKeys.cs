namespace PhloSystemsApi.Models
{
    /// <summary>
    /// Represents the API keys retrieved from the external source.
    /// </summary>
    public class ApiKeys
    {
        /// <summary>
        /// Gets or sets the primary API key.
        /// </summary>
        public string Primary { get; set; }

        /// <summary>
        /// Gets or sets the secondary API key.
        /// </summary>
        public string Secondary { get; set; }
    }

}
