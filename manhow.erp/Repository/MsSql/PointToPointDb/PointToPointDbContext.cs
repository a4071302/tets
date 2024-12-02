using Interface.Repository.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MsSql.PointToPointDb
{
    public class PointToPointDbContext : SqlServerContext, IPointToPointDbContext
    {
        public PointToPointDbContext(string connectionString)
            : base(connectionString, new DefaultSqlServerOptions())
        {
        }

        public PointToPointDbContext(string connectionString, ISqlServerOptions options)
            : base(connectionString, options)
        {
        }
    }
}
