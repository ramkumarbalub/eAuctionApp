using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace eAuctionAPI.Model
{
    public class ProductModel
    {      
        [BsonElement]
        public string productId { get; set; }
        [BsonElement]
        public string productname { get; set; }
        //shortdescription
        [BsonElement]
        public string shortdescription { get; set; }
        //detaileddescription
        [BsonElement]
        public string detaileddescription { get; set; }
        //category
        [BsonElement]
        public string category { get; set; }
        /*
        //startingprice
        [BsonElement]
        public Double startingprice { get; set; }
        //bidenddate
        [BsonElement]
        [BsonDateTimeOptions (Kind = DateTimeKind.Local)]
        public DateTime bidenddate { get; set; }
        */
        [BsonElement]
        public string startingprice { get; set; }
        [BsonElement]
        public string bidenddate { get; set; }
    }
}
