using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eAuctionAPI.Model
{
    public class BuyerModel
    {
        [BsonElement]
        public string firstname { get; set; }
        [BsonElement]
        public string lastname { get; set; }
        [BsonElement]
        public string address { get; set; }
        [BsonElement]
        public string city { get; set; }
        [BsonElement]
        public string state { get; set; }
        [BsonElement]
        public string pin { get; set; }
        [BsonElement]
        public string phone { get; set; }
        [BsonElement]
        public string email { get; set; }
        [BsonElement]
        public string bidamount { get; set; }
        [BsonElement]
        public string productid { get; set; }
    }
}
