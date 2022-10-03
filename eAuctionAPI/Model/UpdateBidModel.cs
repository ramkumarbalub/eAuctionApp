using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eAuctionAPI.Model
{
    public class UpdateBidModel
    {
        public string productId { get; set; }
        public string emailId { get; set; }
        public string bidPrice { get; set; }
    }
}
