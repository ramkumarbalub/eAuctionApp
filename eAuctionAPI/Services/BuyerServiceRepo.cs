using eAuctionAPI.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eAuctionAPI.Services
{
    public class BuyerServiceRepo
    {
        private readonly IMongoCollection<BuyerModel> _buyerCollection;

        public BuyerServiceRepo()
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017/");
            var mongoDatabase = mongoClient.GetDatabase("eAuction");
            _buyerCollection = mongoDatabase.GetCollection<BuyerModel>("buyerv2"); //productv2
        }

        public async Task CreateAsync(BuyerModel newBuyer) =>
           await _buyerCollection.InsertOneAsync(newBuyer);
    }
}
