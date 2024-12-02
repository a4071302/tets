using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repository.MsSql
{
    public interface ISqlBaseContext : IDisposable
    {
        IDbConnection Connection { get; }
        List<T> Query<T>(string sql, object? param = null, IDbTransaction? transaction = null);
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null);
        T First<T>(string sql, object? param = null, IDbTransaction? transaction = null);
        Task<T> FirstAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null);
        int Execute(string sql, object? param = null, IDbTransaction? transaction = null);
        Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null);
        T Execute<T>(string sql, object? param = null, IDbTransaction? transaction = null);
        Task<T> ExecuteAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null);
        IDbTransaction CreateTransition();
        Task<int> InsertAsync<T>(T entity);
        //Task<int> InsertBatchAsync<T>(List<T> entitys, IDbTransaction? transaction = null);
        Task<int> UpdateAsync<T>(T entity, string key, IDbTransaction? transaction = null);
        //Task<int> UpdateBatchAsync<T>(List<T> entity, string key, IDbTransaction? transaction = null);
    }
}
