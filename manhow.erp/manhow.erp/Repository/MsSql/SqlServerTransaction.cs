using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MsSql
{
    public class SqlServerTransaction : IDbTransaction
    {
        private readonly SqlConnection _con;
        private readonly bool _closeConnection;
        private readonly SqlTransaction _transaction;

        public SqlServerTransaction(SqlConnection conn, bool closeConnection = true)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            _con = conn;
            _closeConnection = closeConnection;
            _transaction = conn.BeginTransaction();
        }

        public SqlServerTransaction(SqlConnection conn, IsolationLevel isolationLevel, bool closeConnection = true)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            _con = conn;
            _closeConnection = closeConnection;
            _transaction = conn.BeginTransaction(isolationLevel);
        }

        public IDbConnection Connection => _con;

        public IsolationLevel IsolationLevel => _transaction.IsolationLevel;

        internal SqlTransaction Transaction => _transaction;

        public void Commit()
        {
            _transaction.Commit();
            CheckCloseConnection();
        }

        public void Rollback()
        {
            _transaction.Rollback();
            CheckCloseConnection();
        }

        public void Dispose()
        {
            _transaction.Dispose();
            CheckCloseConnection();
        }
        private void CheckCloseConnection()
        {
            if (_closeConnection)
                _con.Close();
        }

    }
}
