using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ordering.entities
{
    public class Order
    {
        public string Id { get; set; }

        public string StartDay { get; set; }

        public string EndDay { get; set; }

        public string RoomNo { get; set; }

        public string MemberId { get; set; }

        public string HotelId { get; set; }

        public string CreateDay { get; set; }
    }
}
