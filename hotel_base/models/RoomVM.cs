using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace hotel_base.models
{
    public class RoomVM
    {
        public string Id { get; set; }

        public string No { get; set; }

        public int Floor { get; set; }

        public string HotelId { get; set; }
    }
}
