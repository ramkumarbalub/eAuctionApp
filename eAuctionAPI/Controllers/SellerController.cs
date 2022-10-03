using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Microsoft.AspNetCore.Authorization;
using eAuctionAPI.Model;
using eAuctionAPI.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System.Text;

namespace eAuctionAPI.Controllers
{
   // [Authorize]
    
    //[Route("api/v1/[controller]/[action]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private SellerServiceRepo _sellerServiceRepo;
        public IConfiguration _configuration { get; }
        public SellerController(IConfiguration config)
        {
            _sellerServiceRepo = new SellerServiceRepo();
            _configuration = config;
        }

        /*
        public SellerController(SellerServiceRepo sellerServiceRepo)
        {
            _sellerServiceRepo = sellerServiceRepo;
        }
        */
        /*
        [ActionName("PPV1")]
        [HttpPost]
        public void PPV1(ProductModel pmo)
        {
            string serverURI = "mongodb://localhost:27017/";
            string database = "eAuction";
            string collectionName = "productv2";
            MongoClient client = new MongoClient(serverURI);
        }
        */
        /*
        //[ActionName("getproduct")]
        [HttpGet]
        [Route("/v1/Seller/getproduct")]
        public async Task<List<ProductModel>> getproduct()
        => await _sellerServiceRepo.GetProductsAsync();
        */
        
        [ActionName("post-product")]
        [HttpPost]
        [Route("/v1/Seller/Post")]
        public void Post(ProductModel newProduct)
        {
            //await _sellerServiceRepo.CreateAsync(newProduct);

            string conString = "Endpoint=sb://eauctionsbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=UHnqYuXj0sgD6jvMupVjr84ns1oudG1VmDgWFhCfcW4=";

            string conString_appsetting = _configuration.GetSection("ConnectionStrings").GetSection("QueueConnectionString").Value;
            IQueueClient queueClient = new QueueClient(conString, "addproductq");
            //Convert the modal data into Json using JsonConvert.SerializeObject
            var productJSON = JsonConvert.SerializeObject(newProduct);
            //Create a message based on above Json
            var productMessage = new Message(Encoding.UTF8.GetBytes(productJSON))
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = "application/json"
            };
            queueClient.SendAsync(productMessage).ConfigureAwait(false);


           // return NoContent();
        }
        /*
        [ActionName("add-product")]
        [HttpPost]
        [Route("/")]
        public void AddProduct(string ProductName, string ShortDesc, string DetailedDesc, string Category, Double StartingPrice, DateTime BidEndDate)
        {
            try
            {
                //Product Name
                //Short Description
                //Detailed Description
                //Category
                //Starting Price
                //Bid end date

                //Save the product information in mongo database

                //Connect the MongoDB
                string serverURI = "mongodb://localhost:27017/";
                string database = "eAuction";
                string collectionName = "productv2";
                MongoClient client = new MongoClient(serverURI);

                //Connect the Particular database
                MongoServer myMongoServer = client.GetServer();

                MongoDefaults.MaxConnectionIdleTime = TimeSpan.FromMinutes(3);

                if (myMongoServer.State == MongoServerState.Disconnected)
                    myMongoServer.Connect();

                MongoDatabase myMongoDatabase = myMongoServer.GetDatabase(database);

                //Access the particular collection
                MongoCollection<BsonDocument> myMongoCollection = myMongoDatabase.GetCollection<BsonDocument>(collectionName);


                #region Insert document
                long recordCount = myMongoCollection.Count();

                myMongoCollection.Insert(new BsonDocument
                    {

                        { "productId", $"prd{++recordCount}" },
                        { "productname", ProductName},
                        { "shortdescription", "Necklace is an" },
                        { "detaileddescription", "Long Necklace" },
                        { "category", "2" },
                        { "startingprice", "100.00" },
                        { "bidenddate", "2022-08-25" }

                    });
                #endregion
                //Get all the Products 
                //var myAllProducts = myMongoCollection.FindAll();

                ////Iterate each product in the above collection
                //foreach (BsonDocument bsonProduct in myAllProducts)
                //{
                //    var prod = bsonProduct.ToJson();

                //}

                //Construct the incremented ProductId
                //int myAllProducts..Count()++;
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.InnerException}, {ex.Message}");
            }
        }

        */
        //[ActionName("getProductInfo")]
        [HttpGet]
        [Route("/v1/Seller/GetProductInfo")]
        public string GetProductInfo(string productId)
        {
            try
            {
                //Connect the MongoDB
                string serverURI = "mongodb://eauction-cosmos-mongo:YEzhfAB9jZKe0TPmWkcXtm31bsnPA97bH4nWjJmi9fOcuNLQnbyEt4XuFLnJeQWhMeOCKSbjl7zD5JEeVLsV9Q==@eauction-cosmos-mongo.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@eauction-cosmos-mongo@";
                string database = "eAuction";
                string collectionName = "products";
                MongoClient client = new MongoClient(serverURI);

                //Connect the Particular database
                MongoServer myMongoServer = client.GetServer();

                MongoDefaults.MaxConnectionIdleTime = TimeSpan.FromMinutes(3);

                if (myMongoServer.State == MongoServerState.Disconnected)
                    myMongoServer.Connect();

                MongoDatabase myMongoDatabase = myMongoServer.GetDatabase(database);

                //Access the particular collection
                MongoCollection<BsonDocument> myMongoCollection = myMongoDatabase.GetCollection<BsonDocument>(collectionName);

                //List all the bids against the product             
                var BidsList = myMongoCollection.Find(Query.EQ("productId", productId))
                    .SetFields(Fields.Include("productId", "productname", "shortdescription",
                    "detaileddescription", "category", "startingprice", "bidenddate").Exclude("_id")).ToList();

                
                return BidsList.FirstOrDefault().ToJson();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        

        //[ActionName("listProduct")]
        [HttpGet]
        [Route("/v1/Seller/ListProductName")]
        public string ListProductName()
        {
            try
            {
                //Connect the MongoDB
                
                //string serverURI = "mongodb://localhost:27017/";
                //string database = "eAuction";
                //string collectionName = "productv2";
                

                string serverURI = "mongodb://eauction-cosmos-mongo:YEzhfAB9jZKe0TPmWkcXtm31bsnPA97bH4nWjJmi9fOcuNLQnbyEt4XuFLnJeQWhMeOCKSbjl7zD5JEeVLsV9Q==@eauction-cosmos-mongo.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@eauction-cosmos-mongo@";
                string database = "eAuction";
                string collectionName = "products";
                MongoClient client = new MongoClient(serverURI);

                //Connect the Particular database
                MongoServer myMongoServer = client.GetServer();

                MongoDefaults.MaxConnectionIdleTime = TimeSpan.FromMinutes(3);

                if (myMongoServer.State == MongoServerState.Disconnected)
                    myMongoServer.Connect();

                MongoDatabase myMongoDatabase = myMongoServer.GetDatabase(database);

                //Access the particular collection
                MongoCollection<BsonDocument> myMongoCollection = myMongoDatabase.GetCollection<BsonDocument>(collectionName);

                //List all the bids against the product             
                var BidsList = myMongoCollection.FindAll()
                    .SetFields(Fields.Include("productId", "productname").Exclude("_id")).ToList();

                string prodJson = string.Empty;
                //Iterate each product in the above collection
                foreach (BsonDocument bsonProduct in BidsList)
                {
                    //var prod = bsonProduct.ToJson();
                    prodJson = bsonProduct.ToJson();
                }
                return BidsList.ToJson();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /*
        /// <summary>
        /// Get the bids placed against a product
        /// </summary>
        /// <param name="productId"></param>
        //[ActionName("show-bids")]
        //[Route("{productId}")]
        [HttpGet]
        [Route("/v1/Seller/ShowBid")]
        public string ShowBid(string productId)
        {            
            try
            {
                //Connect the MongoDB
                string serverURI = "mongodb://localhost:27017/";
                string database = "eAuction";
                string collectionName = "buyers";
                MongoClient client = new MongoClient(serverURI);

                //Connect the Particular database
                MongoServer myMongoServer = client.GetServer();

                MongoDefaults.MaxConnectionIdleTime = TimeSpan.FromMinutes(3);

                if (myMongoServer.State == MongoServerState.Disconnected)
                    myMongoServer.Connect();

                MongoDatabase myMongoDatabase = myMongoServer.GetDatabase(database);

                //Access the particular collection
                MongoCollection<BsonDocument> myMongoCollection = myMongoDatabase.GetCollection<BsonDocument>(collectionName);

                //List all the bids against the product             
                var BidsList = myMongoCollection.Find(Query.EQ("productId", productId))
                    .SetFields(Fields.Include("productId").Exclude("_id")).ToList();

                string prodJson = string.Empty;  
                //Iterate each product in the above collection
                foreach (BsonDocument bsonProduct in BidsList)
                {
                    //var prod = bsonProduct.ToJson();
                    prodJson = bsonProduct.ToJson();
                }
                return prodJson;
            }
            catch (Exception)
            {

                throw;
            }           
        }
        
        
        /// <summary>
        /// Delete the product if no bids are placed against the product
        /// </summary>
        /// <param name="productId"></param>
        [ActionName("delete")]
        [Route("{productId}")]
        [HttpDelete]
        public void DeleteProduct(string productId)
        {
            try
            {
                //Connect the MongoDB
                string serverURI = "mongodb://localhost:27017/";
                string database = "eAuction";
                string collectionName = "products";
                MongoClient client = new MongoClient(serverURI);

                //Connect the Particular database
                MongoServer myMongoServer = client.GetServer();

                MongoDefaults.MaxConnectionIdleTime = TimeSpan.FromMinutes(3);

                if (myMongoServer.State == MongoServerState.Disconnected)
                    myMongoServer.Connect();

                MongoDatabase myMongoDatabase = myMongoServer.GetDatabase(database);

                //Access the particular collection
                MongoCollection<BsonDocument> myMongoCollection = myMongoDatabase.GetCollection<BsonDocument>(collectionName);

               //Delete the product based on ProductId
                myMongoCollection.Remove(Query.EQ("ProductId", productId));
            }
            catch (Exception)
            {

                throw;
            }
        }
        */

    }
}
