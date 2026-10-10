using AspNetCoreMultiApp.Api.Database;
using AspNetCoreMultiApp.Api.Repositories;

namespace AspNetCoreMultiApp.Api.Services
{
    public class ProductService: IProductService
    {
        private readonly IProductRepository _repo;
        public ProductService(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _repo.AddAsync(product);
            await _repo.SaveChangesAsync();
            return product;
        }

        public async Task<List<Product>> GetAllProductsAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }
    }
}
