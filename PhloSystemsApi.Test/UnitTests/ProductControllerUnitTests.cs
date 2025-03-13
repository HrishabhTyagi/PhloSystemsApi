using FakeItEasy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PhloSystemsApi.Controllers;
using PhloSystemsApi.Models;
using PhloSystemsApi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhloSystemsApi.Test.UnitTests
{
    [TestFixture]
    public class ProductControllerUnitTests
    {
        private IProductService _productService;
        private ILogger<ProductController> _logger;
        private ProductController _controller;

        [SetUp]
        public void SetUp()
        {
            _productService = A.Fake<IProductService>();
            _logger = A.Fake<ILogger<ProductController>>();
            _controller = new ProductController(_productService, _logger);
        }

        [Test]
        public async Task FilterProducts_ShouldReturnFilteredProducts_WhenValidFiltersAreProvided()
        {
            // Arrange
            var mockProducts = new List<Product>
            {
                new Product { Title = "Product1", Price = 50, Sizes = new List<string> { "S", "M" }, Description = "Test description 1" },
                new Product { Title = "Product2", Price = 100, Sizes = new List<string> { "L", "XL" }, Description = "Test description 2" }
            };

            A.CallTo(() => _productService.GetProductsAsync()).Returns(Task.FromResult(mockProducts));

            decimal? minPrice = 60;
            decimal? maxPrice = 120;
            string size = "L";
            string highlight = "description";

            // Act
            var result = await _controller.FilterProducts(minPrice, maxPrice, size, highlight) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

            var response = result.Value as ProductResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Products.Count, Is.EqualTo(1));
            Assert.That(response.Products.First().Title, Is.EqualTo("Product2"));
            Assert.IsTrue(response.Products.First().Description.Contains("<em>description</em>"));
        }

        [Test]
        public async Task FilterProducts_ShouldReturnAllProducts_WhenNoFiltersAreProvided()
        {
            // Arrange
            var mockProducts = new List<Product>
            {
                new Product { Title = "Product1", Price = 50, Sizes = new List<string> { "S", "M" }, Description = "Test description 1" },
                new Product { Title = "Product2", Price = 100, Sizes = new List<string> { "L", "XL" }, Description = "Test description 2" }
            };

            A.CallTo(() => _productService.GetProductsAsync()).Returns(Task.FromResult(mockProducts));

            // Act
            var result = await _controller.FilterProducts(null, null, null, null) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

            var response = result.Value as ProductResponse;
            Assert.IsNotNull(response);
            Assert.That(response.Products.Count, Is.EqualTo(mockProducts.Count));
        }

        [Test]
        public async Task FilterProducts_ShouldReturnEmptyList_WhenNoProductsMatchFilters()
        {
            // Arrange
            var mockProducts = new List<Product>
            {
                new Product { Title = "Product1", Price = 50, Sizes = new List<string> { "S", "M" }, Description = "Test description 1" },
                new Product { Title = "Product2", Price = 100, Sizes = new List<string> { "L", "XL" }, Description = "Test description 2" }
            };

            A.CallTo(() => _productService.GetProductsAsync()).Returns(Task.FromResult(mockProducts));

            decimal? minPrice = 150; // No product matches this filter
            decimal? maxPrice = 200;

            // Act
            var result = await _controller.FilterProducts(minPrice, maxPrice, null, null) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

            var response = result.Value as ProductResponse;
            Assert.IsNotNull(response);
            Assert.IsEmpty(response.Products);
        }

        [Test]
        public async Task FilterProducts_ShouldHandleExceptionAndReturn500Status()
        {
            // Arrange
            A.CallTo(() => _productService.GetProductsAsync()).Throws<Exception>();

            // Act
            var result = await _controller.FilterProducts(null, null, null, null) as ObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
            Assert.That(result.Value, Is.EqualTo("An error occurred while processing your request."));
        }

        [Test]
        public async Task FilterProducts_ShouldReturnFilterMetadataCorrectly()
        {
            // Arrange
            var mockProducts = new List<Product>
            {
                new Product { Title = "Product1", Price = 50, Sizes = new List<string> { "S", "M" }, Description = "Test description 1" },
                new Product { Title = "Product2", Price = 100, Sizes = new List<string> { "L", "XL" }, Description = "Test description 2" }
            };

            A.CallTo(() => _productService.GetProductsAsync()).Returns(Task.FromResult(mockProducts));

            // Act
            var result = await _controller.FilterProducts(null, null, null, null) as OkObjectResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(StatusCodes.Status200OK));

            var response = result.Value as ProductResponse;
            Assert.IsNotNull(response);

            var metadata = response.filterMetaData;
            Assert.IsNotNull(metadata);
            Assert.That(metadata.MinPrice, Is.EqualTo(50));
            Assert.That(metadata.MaxPrice, Is.EqualTo(100));
            CollectionAssert.AreEquivalent(new[] { "S", "M", "L", "XL" }, metadata.Sizes);
        }
    }
}
