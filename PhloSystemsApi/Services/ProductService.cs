
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PhloSystemsApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhloSystemsApi.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductService> _logger;

        public ProductService(HttpClient httpClient, ILogger<ProductService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Fetches products from the external URL and deserializes them
        /// </summary>
        /// <returns>A list of products from the source URL.</returns>
        public async Task<List<Product>> GetProductsAsync()
        {
            try
            {
                _logger.LogInformation("Fetching products from the external source...");
                var response = await _httpClient.GetAsync("JucRNpWs");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch products. Status code: {StatusCode}", response.StatusCode);
                    throw new Exception("Unable to fetch products from the external source.");
                }

                var content = await response.Content.ReadAsStringAsync();
                _logger.LogDebug("Raw product data: {Content}", content);

                // Deserialize
                var apiResponse = JsonConvert.DeserializeObject<FilterResponse>(content);
                if (apiResponse?.Products == null)
                {
                    _logger.LogWarning("No products found in the response.");
                    return new List<Product>();
                }

                _logger.LogInformation("Successfully fetched and parsed products.");
                return apiResponse.Products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching products.");
                throw;
            }
        }
    }

}
