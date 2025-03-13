namespace PhloSystemsApi.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using PhloSystemsApi.Models;
    using PhloSystemsApi.Services;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    /// <summary>
    /// Controller to handle product-related operations.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly ILogger<ProductController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductController"/> class.
        /// </summary>
        /// <param name="productService">The product service to fetch and filter products.</param>
        /// <param name="logger">The logger for capturing logs.</param>
        public ProductController(IProductService productService, ILogger<ProductController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// Gets filtered products based on optional query parameters.
        /// </summary>
        /// <param name="minPrice">The minimum price to filter products.</param>
        /// <param name="maxPrice">The maximum price to filter products.</param>
        /// <param name="size">The size to filter products.</param>
        /// <param name="highlight">Comma-separated words to highlight in descriptions.</param>
        /// <returns>A filtered list of products with additional filter metadata.</returns>
        [HttpGet("filter")]
        public async Task<IActionResult> FilterProducts(
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? size,
            [FromQuery] string? highlight)
        {
            if (minPrice < 0)
            {
                return BadRequest("minPrice cannot be less than 0.");
            }

            if (maxPrice < 0)
            {
                return BadRequest("maxPrice cannot be less than 0.");
            }

            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            {
                return BadRequest("minPrice cannot be greater than maxPrice.");
            }

            try
            {
                _logger.LogInformation("Fetching products with filters: minPrice={MinPrice}, maxPrice={MaxPrice}, size={Size}, highlight={Highlight}", minPrice, maxPrice, size, highlight);

                var products = await _productService.GetProductsAsync();

                // Apply filters
                var filteredProducts = products
                    .Where(p => (!minPrice.HasValue || p.Price >= minPrice) &&
                                (!maxPrice.HasValue || p.Price <= maxPrice) &&
                                (string.IsNullOrEmpty(size) || p.Sizes.Contains(size)))
                    .ToList();

                // Highlight descriptions
                if (!string.IsNullOrEmpty(highlight))
                {
                    var highlightWords = highlight.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var product in filteredProducts)
                    {
                        foreach (var word in highlightWords)
                        {
                            product.Description = product.Description.Replace(word, $"<em>{word}</em>", StringComparison.OrdinalIgnoreCase);
                        }
                    }
                }

                _logger.LogInformation("Successfully filtered and processed products.");

                return Ok(new ProductResponse
                {
                    filterMetaData = new FilterMetadata
                    {
                        MinPrice = products.Min(p => p.Price),
                        MaxPrice = products.Max(p => p.Price),
                        Sizes = products.SelectMany(p => p.Sizes).Distinct().OrderBy(s => s).ToArray(),
                        MostCommonWords = GetMostCommonWords(products.Select(p => p.Description), 10, 5)
                    },
                    Products = filteredProducts
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while filtering products.");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// Extracts the most common words from a collection of strings.
        /// </summary>
        /// <param name="descriptions">The collection of descriptions to analyze.</param>
        /// <param name="topCount">The number of top words to extract.</param>
        /// <param name="skipCommonCount">The number of most common words to skip.</param>
        /// <returns>An array of the most common words.</returns>
        private static string[] GetMostCommonWords(IEnumerable<string> descriptions, int topCount, int skipCommonCount)
        {
            var commonWords = new[] { "the", "and", "of", "to", "a", "with", "in", "this", "or", "that" };
            var wordCounts = descriptions
                .SelectMany(d => d.Split(new[] { ' ', '.', ',', ';', '!' }, StringSplitOptions.RemoveEmptyEntries))
                .Select(w => w.ToLowerInvariant())
                .Where(w => !commonWords.Contains(w))
                .GroupBy(w => w)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.Count());

            return wordCounts.Keys.Skip(skipCommonCount).Take(topCount).ToArray();
        }
    }

}


