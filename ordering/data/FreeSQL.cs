using ordering.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ordering.data
{
    public static class FreeSQL
    {
        private static IFreeSql _freesql;

        static FreeSQL()
        {
            _freesql = new FreeSql.FreeSqlBuilder()
                          .UseConnectionString(FreeSql.DataType.Sqlite, "Data Source=sqlite.db")
                          .Build();
            try
            {
                _freesql.CodeFirst.SyncStructure<Order>();

            }
            catch 
            {
            }
            if (!_freesql.Select<Order>().Any())
            {
                var order = new Order
                {
                    Id = "OD001",
                    StartDay = "2021-05-01",
                    EndDay = "2021-05-02",
                    RoomNo = "1001",
                    MemberId = "M001",
                    HotelId = "H8001",
                    CreateDay = "2021-05-01"
                };
                var oder1 = new Order
                {
                    Id = "OD002",
                    StartDay = "2021-05-03",
                    EndDay = "2021-05-04",
                    RoomNo = "1002",
                    MemberId = "M002",
                    HotelId = "H8001",
                    CreateDay = "2021-05-03"
                };
                _freesql.Insert(order).ExecuteAffrows();
                _freesql.Insert(oder1).ExecuteAffrows();
            }
        }

        public static IFreeSql Instance => _freesql;

    }
}
