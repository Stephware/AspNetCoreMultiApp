using AspNetCoreMultiApp.Api.Database;

namespace AspNetCoreMultiApp.Api.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAllProductsAsync();
        Task<Product?> GetByIdAsync(int id);
        Task<Product> CreateAsync(Product product);
    }
}
