using ProductService.Interfaces;
using ProductService.Models;
using System.ComponentModel.DataAnnotations;

namespace ProductService.Services
{
    public class ProductManagementService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductManagementService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task AddProductAsync(Product product)
        {
            await _productRepository.AddProductAsync(product);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var procucts = await _productRepository.GetAllProductsAsync();
            return procucts;
        }

        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            var product = await _productRepository.GetProductByIdAsync(productId);
            return product;
        }

        public async Task UpdateProductAsync(Product product)
        {
            await _productRepository.UpdateProductAsync(product);
        }
    }
}
