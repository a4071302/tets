using Interface.Repository.MsSql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.MsSql.ErpDb
{
    public class ErpDbContext : SqlServerContext, IErpDbContext
    {
        public ErpDbContext(string connectionString)
            : base(connectionString, new DefaultSqlServerOptions())
        {
        }

        public ErpDbContext(string connectionString, ISqlServerOptions options)
            : base(connectionString, options)
        {
        }
    }
}
