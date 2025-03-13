using FakeItEasy;
using PhloSystemsApi.Models;
using PhloSystemsApi.Services;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace PhloSystemsApi.Test.IntegrationTests
{
    [TestFixture]
    public class ProductServiceIntegrationTests
    {
        private HttpClient _httpClient;
        private ILogger<ProductService> _logger;
        private ProductService _productService;

        [SetUp]
        public void SetUp()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://pastebin.com/raw/") };
            _logger = new LoggerFactory().CreateLogger<ProductService>();
            _productService = new ProductService(_httpClient, _logger);
        }

        [Test]
        public async Task GetProductsAsync_ShouldReturnProducts()
        {
            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.IsNotNull(result);
        }

        [Test]
        public void GetProductsAsync_ShouldThrowException_WhenServerIsUnreachable()
        {
            // Arrange
            _httpClient.BaseAddress = new Uri("http://invalidserver.com/");

            // Act & Assert
            Assert.ThrowsAsync<HttpRequestException>(async () => await _productService.GetProductsAsync());
        }
    }
}