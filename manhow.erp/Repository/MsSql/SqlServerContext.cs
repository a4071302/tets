using Dapper;
using Interface.Repository.MsSql;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MsSql
{
    public class SqlServerContext : ISqlBaseContext
    {
        private readonly ISqlServerOptions _options;
        private readonly string _connectionString;
        private readonly SqlConnection _connection;

        public SqlServerContext(string connectionString, ISqlServerOptions options)
        {
            _connectionString = connectionString;
            _options = options;
            _connection = new SqlConnection(_connectionString);
        }

        public IDbConnection Connection => _connection;

        public IDbTransaction CreateTransition()
        {
            return new SqlServerTransaction(_connection, _options.TransactionAutoCloseConnection);
        }

        public int Execute(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return _connection.Execute(sql, param, CheckIsProxyTransaction(transaction), _options.CommandTimeout);
        }

        public T Execute<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return _connection.ExecuteScalar<T>(sql, param, CheckIsProxyTransaction(transaction), _options.CommandTimeout);
        }

        public Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            try
            {
                return _connection.ExecuteAsync(sql, param, CheckIsProxyTransaction(transaction), _options.CommandTimeout);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public Task<T> ExecuteAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return _connection.ExecuteScalarAsync<T>(sql, param, CheckIsProxyTransaction(transaction), _options.CommandTimeout);
        }

        public T First<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return _connection.QueryFirst<T>(sql, param, CheckIsProxyTransaction(transaction), _options.CommandTimeout);
        }

        public Task<T> FirstAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return _connection.QueryFirstAsync<T>(sql, param, CheckIsProxyTransaction(transaction), _options.CommandTimeout);
        }

        public List<T> Query<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return _connection.Query<T>(sql, param, CheckIsProxyTransaction(transaction), commandTimeout: _options.CommandTimeout).AsList();
        }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null)
        {
            return _connection.QueryAsync<T>(sql, param, CheckIsProxyTransaction(transaction), commandTimeout: _options.CommandTimeout);
        }

        public void Dispose()
        {
            if (_connection.State != ConnectionState.Closed)
                _connection.Close();

            _connection.Dispose();
        }

        private IDbTransaction? CheckIsProxyTransaction(IDbTransaction? transaction)
        {
            return transaction is SqlServerTransaction sqt
                ? sqt.Transaction
                : transaction;
        }

        public async Task<int> InsertAsync<T>(T entity)
        {
            var tmp = new List<T>();
            tmp.Add(entity);
            string sql = GenerateInsertSql(tmp);
            return await ExecuteAsync(sql, entity);
        }

        //public async Task<int> InsertBatchAsync<T>(List<T> entitys, IDbTransaction? transaction = null)
        //{
        //    int result = 0;
        //    string sql = GenerateInsertSql(entitys);
        //    var tmps = MyLinq.ChunkBy(entitys, 200);
        //    foreach (var item in tmps)
        //    {
        //        result += await ExecuteAsync(sql, item, transaction);
        //    }

        //    return result;
        //}

        public async Task<int> UpdateAsync<T>(T entity, string key, IDbTransaction? transaction = null)
        {
            var tmp = new List<T>();
            tmp.Add(entity);
            string sql = GenerateUpdateSql(tmp, key);
            return await ExecuteAsync(sql, entity, transaction);
        }

        /// <summary>
        /// 生成插入SQL 指令
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private string GenerateInsertSql<T>(List<T> entities)
        {
            if (entities == null || !entities.Any())
            {
                throw new ArgumentException("Entity list cannot be null or empty", nameof(entities));
            }

            var properties = typeof(T).GetProperties();
            var nonNullProperties = properties.Where(p => entities.Any(e => p.GetValue(e) != null));

            var columnNames = string.Join(", ", nonNullProperties.Select(p => $"[{p.Name}]"));
            var parameterNames = string.Join(", ", nonNullProperties.Select(p => "@" + p.Name));

            string tableName = typeof(T).Name;
            return $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameterNames})";

        }

        private string GenerateUpdateSql<T>(List<T> entities, string keyColumn)
        {
            if (entities == null || !entities.Any())
            {
                throw new ArgumentException("Entity list cannot be null or empty", nameof(entities));
            }

            var properties = typeof(T).GetProperties();

            // Filter out key column and properties where all values are null
            var nonKeyNonNullProperties = properties.Where(p =>
                p.Name != keyColumn && entities.Any(e => p.GetValue(e) != null));

            // Generate the set clause only for properties with non-null values
            var setClause = string.Join(", ", nonKeyNonNullProperties.Select(p => $"[{p.Name}] = @{p.Name}"));

            string tableName = typeof(T).Name;
            return $"UPDATE {tableName} SET {setClause} WHERE [{keyColumn}] = @{keyColumn}";
        }

        //public async Task<int> UpdateBatchAsync<T>(List<T> entitys, string key, IDbTransaction? transaction = null)
        //{
        //    int result = 0;
        //    string sql = GenerateUpdateSql(entitys, key);
        //    var tmps = MyLinq.ChunkBy(entitys, 200);
        //    foreach (var item in tmps)
        //    {
        //        result += await ExecuteAsync(sql, item, transaction);
        //    }

        //    return result;
        //}
    }

    public class DefaultSqlServerOptions : ISqlServerOptions
    {
        public int? CommandTimeout { get; set; } = null;
        public bool TransactionAutoCloseConnection { get; set; } = true;
    }
}
