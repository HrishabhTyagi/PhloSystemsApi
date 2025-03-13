using FakeItEasy;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using PhloSystemsApi.Models;
using PhloSystemsApi.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace PhloSystemsApi.Test.UnitTests
{
    [TestFixture]
    public class ProductServiceTests
    {
        private ProductService _productService;
        private HttpClient _httpClient;
        private ILogger<ProductService> _logger;
        private FakeHttpMessageHandler _fakeHttpMessageHandler;

        [SetUp]
        public void SetUp()
        {
            _fakeHttpMessageHandler = new FakeHttpMessageHandler();
            _httpClient = new HttpClient(_fakeHttpMessageHandler) { BaseAddress = new Uri("https://pastebin.com/raw/") };
            _logger = A.Fake<ILogger<ProductService>>();
            _productService = new ProductService(_httpClient, _logger);
        }

        [Test]
        public async Task GetProductsAsync_ShouldReturnProducts_WhenResponseIsSuccessful()
        {
            // Arrange
            var responseContent = "{\"Products\": [{\"Title\": \"Product1\", \"Price\": 10.5, \"Sizes\": [\"Small\"], \"Description\": \"Sample product\"}]}";
            _fakeHttpMessageHandler.SetResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Title, Is.EqualTo("Product1"));
        }

        [Test]
        public void GetProductsAsync_ShouldThrowException_WhenResponseIsUnsuccessful()
        {
            // Arrange
            _fakeHttpMessageHandler.SetResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest
            });

            // Act & Assert
            var exception = Assert.ThrowsAsync<Exception>(async () => await _productService.GetProductsAsync());
            Assert.That(exception.Message, Is.EqualTo("Unable to fetch products from the external source."));
        }

        [Test]
        public async Task GetProductsAsync_ShouldReturnEmptyList_WhenProductsAreNull()
        {
            // Arrange 
            var responseContent = "{\"Products\": null}";
            _fakeHttpMessageHandler.SetResponse(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseContent)
            });

            // Act
            var result = await _productService.GetProductsAsync();

            // Assert
            Assert.IsNotNull(result);
            Assert.That(result.Count, Is.EqualTo(0));
        }

    }

    // Custom FakeHttpMessageHandler for mocking HTTP responses
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private HttpResponseMessage _response;

        public void SetResponse(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }

}