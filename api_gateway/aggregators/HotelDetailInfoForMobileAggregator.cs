using Microsoft.AspNetCore.Http;
using Ocelot.Middleware;
using Ocelot.Multiplexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;

namespace api_gateway.aggregators
{
    public class HotelDetailInfoForMobileAggregator : IDefinedAggregator
    {
        public async Task<DownstreamResponse> Aggregate(List<HttpContext> responses)
        {
            dynamic hotelInfo = new ExpandoObject();
            List<dynamic> rooms = new List<dynamic>();
            foreach (var context in responses)
            {
                if ((context.Items["DownstreamRoute"] as dynamic).Key == "hotel_base_info")
                {
                    var respContent = await context.Items.DownstreamResponse().Content.ReadAsStringAsync();
                    hotelInfo = JsonConvert.DeserializeObject<dynamic>(respContent);
                }
                if ((context.Items["DownstreamRoute"] as dynamic).Key == "hotel_rooms")
                {
                    var respContent = await context.Items.DownstreamResponse().Content.ReadAsStringAsync();
                    rooms = JsonConvert.DeserializeObject<List<dynamic>>(respContent);
                }
            }

            dynamic newResponse = new ExpandoObject();
            newResponse.hotel = new { 
                hotelInfo.id,
                hotelInfo.name
            };
            newResponse.rooms = rooms.Select(x => new { 
                x.id,
                x.no
            });

            var stringContent = new StringContent(JsonConvert.SerializeObject(newResponse));

            return new DownstreamResponse(
                stringContent, 
                System.Net.HttpStatusCode.OK, 
                responses.SelectMany(x => x.Items.DownstreamResponse().Headers).ToList(),
                "OK");
        }
    }
}
