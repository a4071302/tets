using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repository.MsSql.ManhowErp
{
    public interface ITestManHowRepository
    {

        /// <summary>
        /// 取得有效資料
        /// </summary>
        /// <param name="typeCode"></param>
        /// <returns></returns>
        Task<List<TestCarPallet>> GetTestCarPallet(string typeCode);
    }
}
