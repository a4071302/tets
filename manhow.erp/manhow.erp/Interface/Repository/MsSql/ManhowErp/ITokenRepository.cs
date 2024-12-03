using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.ManhowErpTable;


namespace Interface.Repository.MsSql.ManhowErp
{
    /// <summary>
    /// TOKEN Table CRUD
    /// </summary>
    public interface ITokenRepository
    {
        /// <summary>
        /// 取得所有Token
        /// </summary>
        /// <returns></returns>
        Task<List<Token>> GetAll();

        /// <summary>
        /// 單筆新增
        /// </summary>
        /// <param name="require"></param>
        /// <returns></returns>
        Task<int> Insert(Token require);

        /// <summary>
        /// 依使用者更新
        /// </summary>
        /// <param name="require"></param>
        /// <returns></returns>
        Task<int> UpdateByUserId(Token require);

        /// <summary>
        /// 依使用者編號(帳號)刪除
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<int> DeleteByUserId(string userId);
    }
}
