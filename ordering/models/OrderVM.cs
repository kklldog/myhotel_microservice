using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ordering.models
{
    public class OrderVM
    {
        public string Id { get; set; }

        public string StartDay { get; set; }

        public string EndDay { get; set; }

        public string RoomNo { get; set; }

        public string MemberId { get; set; }

        public MemberVM Member { get; set; }

        public string HotelId { get; set; }

        public string CreateDay { get; set; }
    }
}
