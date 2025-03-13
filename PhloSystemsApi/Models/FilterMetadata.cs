namespace PhloSystemsApi.Models
{
    public class ProductResponse
    {
        /// <summary>
        /// Gets or sets the list of products.
        /// </summary>
        public List<Product> Products { get; set; }

        /// <summary>
        /// Gets or sets the filter metadata.
        /// </summary>
        public FilterMetadata filterMetaData { get; set; }
    }

    /// <summary>
    /// Represents metadata for filtering products.
    /// </summary>
    public class FilterMetadata
    {
        /// <summary>
        /// Gets or sets the minimum price of the products.
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// Gets or sets the maximum price of the products.
        /// </summary>
        public decimal MaxPrice { get; set; }

        /// <summary>
        /// Gets or sets the available sizes of the products.
        /// </summary>
        public string[] Sizes { get; set; }

        /// <summary>
        /// Gets or sets the most common words in the product descriptions.
        /// </summary>
        public string[] MostCommonWords { get; set; }
    }
}
