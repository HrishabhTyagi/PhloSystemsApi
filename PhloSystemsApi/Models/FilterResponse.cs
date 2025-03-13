namespace PhloSystemsApi.Models
{
    /// <summary>
    /// Represents the response containing filtered products and API keys.
    /// </summary>
    public class FilterResponse
    {
        /// <summary>
        /// Gets or sets the list of filtered products.
        /// </summary>
        public List<Product> Products { get; set; }

        /// <summary>
        /// Gets or sets the API keys associated with the response.
        /// </summary>
        public ApiKeys ApiKeys { get; set; }
    }
}
