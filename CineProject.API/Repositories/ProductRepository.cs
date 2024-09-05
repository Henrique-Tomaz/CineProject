using CineProject.API.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using StackExchange.Redis;
using MongoDB.Bson.IO;
using Newtonsoft.Json;
using JsonConvert = Newtonsoft.Json.JsonConvert;

namespace CineProject.API.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IMongoCollection<Product> _products;
        private readonly IConnectionMultiplexer _redis;

        public ProductRepository(IOptions<MongoDBSettings> mongoSettings, IConnectionMultiplexer redis)
        {
            var client = new MongoClient(mongoSettings.Value.ConnectionString);
            var database = client.GetDatabase(mongoSettings.Value.DatabaseName);
            _products = database.GetCollection<Product>(mongoSettings.Value.CollectionName);
            _redis = redis;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            var cacheKey = "products";
            var db = _redis.GetDatabase();

            var cachedProducts = await db.StringGetAsync(cacheKey);
            if (!cachedProducts.IsNullOrEmpty)
            {
                return JsonConvert.DeserializeObject<IEnumerable<Product>>(cachedProducts);
            }

            var products = await _products.Find(product => true).ToListAsync();

            await db.StringSetAsync(cacheKey, JsonConvert.SerializeObject(products), TimeSpan.FromMinutes(10));

            return products;
        }

        public async Task<Product?> GetByIdAsync(string id)
        {
            var cacheKey = $"product:{id}";
            var db = _redis.GetDatabase();

            var cachedProduct = await db.StringGetAsync(cacheKey);
            if (!cachedProduct.IsNullOrEmpty)
            {
                return JsonConvert.DeserializeObject<Product>(cachedProduct);
            }

            var product = await _products.Find<Product>(product => product.Id == id).FirstOrDefaultAsync();

            if (product != null)
            {
                await db.StringSetAsync(cacheKey, JsonConvert.SerializeObject(product), TimeSpan.FromMinutes(10));
            }

            return product;
        }

        public async Task CreateAsync(Product product)
        {
            await _products.InsertOneAsync(product);

            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync("products");
        }

        public async Task UpdateAsync(Product product)
        {
            await _products.ReplaceOneAsync(p => p.Id == product.Id, product);

            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync("products");
            await db.KeyDeleteAsync($"product:{product.Id}");
        }

        public async Task DeleteAsync(string id)
        {
            await _products.DeleteOneAsync(p => p.Id == id);

            var db = _redis.GetDatabase();
            await db.KeyDeleteAsync("products");
            await db.KeyDeleteAsync($"product:{id}");
        }
    }
}
