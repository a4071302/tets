using Interface.Repository.MsSql.ManhowErp;
using Interface.Service.RouteQuery;
using Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RouteQuery
{
    public class TestManhowErpService : ITestManhowErpService
    {
        private readonly ITestManHowRepository _testManHowRepository;
        public TestManhowErpService(ITestManHowRepository testManHowRepository)
        {
            _testManHowRepository = testManHowRepository;
            
        }

        public async Task<List<MenuModel<int>>> TestGet()
        {
            var items = await _testManHowRepository.GetTestCarPallet("38E5A41F-EC6C-46A6-A0C6-2D9EB2A6829E");

            return items.Select(x => new MenuModel<int>()
            {
                key = x.SeqNo,
                value = x.Driver
            }).ToList();
        }

    }
}
