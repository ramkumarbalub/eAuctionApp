using eAuctionAPI.Model;
using eAuctionAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eAuctionAPI.Controllers
{
    [Authorize]
    //[Route("api/v1/[controller]/[action]")] //Commented for publish
    [ApiController]
    public class BuyerController : ControllerBase
    {
        private BuyerServiceRepo _buyerServiceRepo;
        public IConfiguration _configuration { get; }
        public BuyerController(IConfiguration config)
        {
            _buyerServiceRepo = new BuyerServiceRepo();
            _configuration = config;
        }
        
        //[ActionName("post-buyer")]
        [HttpPost]
        [Route("/v1/Buyer/PostBuyer")]
        public void PostBuyer(BuyerModel biddingProduct)
        {
            
            //await _buyerServiceRepo.CreateAsync(newProduct);

            //Serialize this message and send to queue
            //buyerq

            //Create azure function on top of buyerq

            //return NoContent();
            


            string conString = "Endpoint=sb://eauctionsbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=UHnqYuXj0sgD6jvMupVjr84ns1oudG1VmDgWFhCfcW4=";

            string conString_appsetting = _configuration.GetSection("ConnectionStrings").GetSection("QueueConnectionString").Value;
            IQueueClient queueClient = new QueueClient(conString, "buyerplacingbidq");
            //Convert the modal data into Json using JsonConvert.SerializeObject
            var productJSON = JsonConvert.SerializeObject(biddingProduct);
            //Create a message based on above Json
            var productMessage = new Message(Encoding.UTF8.GetBytes(productJSON))
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = "application/json"
            };
            queueClient.SendAsync(productMessage).ConfigureAwait(false);

        }
        
        /*
        //[ActionName("GetDocumentsFromAzure")]
        [HttpGet]
        [Route("/")]
        public void GetDocumentsFromAzure()
        {             
            string conString = "Endpoint=sb://eauctionsbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=UHnqYuXj0sgD6jvMupVjr84ns1oudG1VmDgWFhCfcW4=";
            
            string conString_appsetting = _configuration.GetSection("ConnectionStrings").GetSection("QueueConnectionString").Value;
            IQueueClient queueClient = new QueueClient(conString, "productsq");
            //Convert the modal data into Json using JsonConvert.SerializeObject
            var productJSON = JsonConvert.SerializeObject(new ProductModel
            {
                productId = "prdV45",
                productname = "Television",
                shortdescription = "TV",
                detaileddescription = "3D",
                category = "painting",
                startingprice = "100.00",
                bidenddate = "23-09-2022"
            });
            //Create a message based on above Json
            var productMessage = new Message(Encoding.UTF8.GetBytes(productJSON))
            {
                MessageId = Guid.NewGuid().ToString(),
                ContentType = "application/json"
            };
            queueClient.SendAsync(productMessage).ConfigureAwait(false);

            //return string.Empty;
        }
        */
        
        //[ActionName("listbidsagainstproduct")]
        [HttpGet]
        [Route("/v1/Buyer/ListBidsAgainstProduct")]
        public string ListBidsAgainstProduct(string productid)
        {
            try
            {
                //Connect the MongoDB
                
                //string serverURI = "mongodb://localhost:27017/";
                //string database = "eAuction";
                //string collectionName = "buyerv2";
                

                string serverURI = "mongodb://eauction-cosmos-mongo:YEzhfAB9jZKe0TPmWkcXtm31bsnPA97bH4nWjJmi9fOcuNLQnbyEt4XuFLnJeQWhMeOCKSbjl7zD5JEeVLsV9Q==@eauction-cosmos-mongo.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@eauction-cosmos-mongo@";
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
                var BidsList = myMongoCollection.Find(Query.EQ("productid", productid))
                    .SetFields(Fields.Include("bidamount", "firstname", "email",
                    "phone").Exclude("_id")).ToList();


                return BidsList.ToJson();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /*
        //[ActionName("place-bid")]
        [HttpPost]
        [Route("/v1/Buyer/PlaceBid")]
        public void PlaceBid(string FirstName, string LastName, string Address, string City, string State, string Pin, 
            long Phone, string Email, string ProductId, Decimal BidAmount)
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
                MongoCollection<BsonDocument> BuyerCollection = myMongoDatabase.GetCollection<BsonDocument>(collectionName);

                #region Insert document
                //long recordCount = BuyerCollection.Count();

                //
                //  {
                //first_name:"john",
                //last_name:"doe",
                //address:"10, car street",
                //city:"Newyork",
                //state:"USA",
                //pin:"1234-567",
                //phone:1234567890,
                //email:"john.doe@gmail.com",
                //productId:"prd01",
                //BidAmount:10000050.00
                //}
                //
                BuyerCollection.Insert(new BsonDocument
                    {

                        { "first_name", FirstName },
                        { "last_name", LastName},
                        { "address", Address },
                        { "city", City },
                        { "state", State },
                        { "pin", Pin },
                        { "phone", Phone },
                        { "email", Email},
                        { "productId", ProductId},
                        { "BidAmount", BidAmount}

                    });
                #endregion
            }
            catch (Exception)
            {

                throw;
            }
        }
        */

        //[ActionName("delete-bid")]
        [HttpPost]
        [Route("/v1/Buyer/DeleteBuyerBid")]
        public void DeleteBuyerBid(UpdateBidModel DeleteBidInfo)
        {
            try
            {
                
                //string conString = "Endpoint=sb://eauctionsbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=UHnqYuXj0sgD6jvMupVjr84ns1oudG1VmDgWFhCfcW4=";

                //string conString_appsetting = _configuration.GetSection("ConnectionStrings").GetSection("QueueConnectionString").Value;
                //IQueueClient queueClient = new QueueClient(conString, "deletebidq");
                ////Convert the modal data into Json using JsonConvert.SerializeObject
                //var productJSON = JsonConvert.SerializeObject(DeleteBidInfo);
                //Create a message based on above Json
                //var productMessage = new Message(Encoding.UTF8.GetBytes(productJSON))
                //{
                //    MessageId = Guid.NewGuid().ToString(),
                //    ContentType = "application/json"
                //};
                //queueClient.SendAsync(productMessage).ConfigureAwait(false);
                
                string serverURI = "mongodb://eauction-cosmos-mongo:YEzhfAB9jZKe0TPmWkcXtm31bsnPA97bH4nWjJmi9fOcuNLQnbyEt4XuFLnJeQWhMeOCKSbjl7zD5JEeVLsV9Q==@eauction-cosmos-mongo.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@eauction-cosmos-mongo@";
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


                //Update the product based on the ProductId
                myMongoCollection.Remove(Query.And(Query.EQ("productid", DeleteBidInfo.productId), Query.EQ("email", DeleteBidInfo.emailId)));

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        
        //[ActionName("update-bid")]
        [HttpPost]
        [Route("/v1/Buyer/UpdateBid")]
        /// <summary>
        /// Update the bid amount
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="buyerEmailId"></param>
        /// <param name="newBidAmount"></param>
        public void UpdateBid(UpdateBidModel updateBidInfo)
        {
            try
            {
                
                //Connect the MongoDB
                //string serverURI = "mongodb://eauction-cosmos-mongo:YEzhfAB9jZKe0TPmWkcXtm31bsnPA97bH4nWjJmi9fOcuNLQnbyEt4XuFLnJeQWhMeOCKSbjl7zD5JEeVLsV9Q==@eauction-cosmos-mongo.mongo.cosmos.azure.com:10255/?ssl=true&replicaSet=globaldb&retrywrites=false&maxIdleTimeMS=120000&appName=@eauction-cosmos-mongo@";
                //string database = "eAuction";
                //string collectionName = "buyers";
                //MongoClient client = new MongoClient(serverURI);

                ////Connect the Particular database
                //MongoServer myMongoServer = client.GetServer();

                //MongoDefaults.MaxConnectionIdleTime = TimeSpan.FromMinutes(3);

                //if (myMongoServer.State == MongoServerState.Disconnected)
                //    myMongoServer.Connect();

                //MongoDatabase myMongoDatabase = myMongoServer.GetDatabase(database);

                ////Access the particular collection
                //MongoCollection<BsonDocument> myMongoCollection = myMongoDatabase.GetCollection<BsonDocument>(collectionName);                               

                ////Update the product based on the ProductId
                //myMongoCollection.Update(Query.And(Query.EQ("productid", productId), Query.EQ("email", buyerEmailId)), Update.Set("bidamount", newBidAmount));

                

                
                string conString = "Endpoint=sb://eauctionsbus.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=UHnqYuXj0sgD6jvMupVjr84ns1oudG1VmDgWFhCfcW4=";

                string conString_appsetting = _configuration.GetSection("ConnectionStrings").GetSection("QueueConnectionString").Value;
                IQueueClient queueClient = new QueueClient(conString, "updatebidq");
                //Convert the modal data into Json using JsonConvert.SerializeObject
                var productJSON = JsonConvert.SerializeObject(new UpdateBidModel
                {
                    bidPrice = updateBidInfo.bidPrice,
                    emailId = updateBidInfo.emailId,
                    productId = updateBidInfo.productId
                });
                //Create a message based on above Json
                var productMessage = new Message(Encoding.UTF8.GetBytes(productJSON))
                {
                    MessageId = Guid.NewGuid().ToString(),
                    ContentType = "application/json"
                };
                queueClient.SendAsync(productMessage).ConfigureAwait(false);
                
            }
            catch (Exception)
            {

                throw;
            }
        }
        
    }
}
