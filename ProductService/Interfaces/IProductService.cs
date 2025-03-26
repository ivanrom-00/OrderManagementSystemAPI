using ProductService.Models;

namespace ProductService.Interfaces
{
    public interface IProductService
    {
        Task AddProductAsync(Product product);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int productId);
        Task UpdateProductAsync(Product product);
    }
}
