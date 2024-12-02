using Interface.Repository.MsSql;
using Interface.Repository.MsSql.ManhowErp;
using Model.ManhowErpTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MsSql.ErpDb
{
    public class TokenRepository: ITokenRepository
    {
        private readonly IErpDbContext _db;
        public TokenRepository(IErpDbContext db) 
        {
            _db = db;
        }

        public async Task<int> DeleteByUserId(string userId)
        {
            string sql = @"DELETE From Token 
WHERE UserId  = @userId ";
            return await _db.ExecuteAsync(sql, new { userId = userId });
        }

        public async Task<List<Token>> GetAll()
        {
            string sql = @"SELECT  * 
FROM Token ";
            var tmp = await _db.QueryAsync<Token>(sql);
            return tmp.Any() ?tmp.ToList() : new List<Token>();
        }

        public async Task<int> Insert(Token require)
        {
            return await _db.InsertAsync(require);
        }

        public async Task<int> UpdateByUserId(Token require)
        {
            return await _db.UpdateAsync(require, nameof(require.UserId));
        }
    }
}
