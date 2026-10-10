using AspNetCoreMultiApp.Api.Database;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreMultiApp.Api.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
        }

        public async Task<List<Product>> GetAllAsync() 
        {
            var data = await _context.Products.OrderBy(x=>x.Id).ToListAsync();
            return data;
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
