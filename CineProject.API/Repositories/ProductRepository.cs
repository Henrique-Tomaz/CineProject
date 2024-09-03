using CineProject.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CineProject.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;

        public ProductRepository(IOptions<MongoDBSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            var database = client.GetDatabase(settings.Value.DatabaseName);
            _products = database.GetCollection<Product>(settings.Value.CollectionName);
        }

        public async Task<IEnumerable<Product>> GetAllAsync() =>
            await _products.Find(p => true).ToListAsync();

        public async Task<Product> GetByIdAsync(string id) =>
            await _products.Find<Product>(p => p.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(Product product) =>
            await _products.InsertOneAsync(product);

        public async Task UpdateAsync(Product product) =>
            await _products.ReplaceOneAsync(p => p.Id == product.Id, product);

        public async Task DeleteAsync(string id) =>
            await _products.DeleteOneAsync(p => p.Id == id);
    }
}
