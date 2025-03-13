namespace PhloSystemsApi.Models
{
    /// <summary>
    /// Represents a product with a title, price, available sizes, and description.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// Gets or sets the title of the product.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the price of the product.
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Gets or sets the list of available sizes for the product.
        /// </summary>
        public List<string> Sizes { get; set; }

        /// <summary>
        /// Gets or sets the description of the product.
        /// </summary>
        public string Description { get; set; }
    }
}
