using PhloSystemsApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhloSystemsApi.Services
{
    /// <summary>
    /// Interface defining the contract for the Product Service.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Fetches the list of products from the external source.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. 
        /// The task result contains the list of products.</returns>
        Task<List<Product>> GetProductsAsync();
    }
}
