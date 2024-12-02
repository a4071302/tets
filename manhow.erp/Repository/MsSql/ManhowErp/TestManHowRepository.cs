using Interface.Repository.MsSql;
using Interface.Repository.MsSql.ManhowErp;
using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MsSql.ManhowErp
{
    public class TestManHowRepository : ITestManHowRepository
    {
        private readonly IPointToPointDbContext _db;
        private int _chunkSize = 200;

        public TestManHowRepository(IPointToPointDbContext db)
        {
            _db = db;
        }

        public async Task<List<TestCarPallet>> GetTestCarPallet(string typeCode)
        {

            string sql = @"SELECT * 
FROM CarPallet 
WHERE Id = @typeCode;";

            var tmp = await _db.QueryAsync<TestCarPallet>(sql, new
            {
                typeCode
            });

            return tmp.Any() ? tmp.ToList() : new List<TestCarPallet>();

        }
       

    }
}
