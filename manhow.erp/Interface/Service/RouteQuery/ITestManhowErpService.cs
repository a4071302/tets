using Model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Service.RouteQuery
{
    public interface ITestManhowErpService
    {

        Task<List<MenuModel<int>>> TestGet();

    }
}
