using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eAuctionAPI.Model;
using MongoDB.Driver;
using MongoDB.Driver.Builders;


namespace eAuctionAPI.Services
{
    public class SellerServiceRepo
    {
        private readonly IMongoCollection<ProductModel> _productCollection;

        public SellerServiceRepo()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017/");
            var mongoDatabase = mongoClient.GetDatabase("eAuction");
            _productCollection = mongoDatabase.GetCollection<ProductModel>("productv2"); //productv2
        }

        public async Task CreateAsync(ProductModel newProduct) =>        
            await _productCollection.InsertOneAsync(newProduct);

        public async Task<List<ProductModel>> GetProductsAsync()
    =>  await _productCollection.Find(x => !x.productId.Equals("abc")).ToListAsync();
        
    }
}
