using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Repository.MsSql
{
    public interface ISqlServerOptions
    {
        public int? CommandTimeout { get; set; }
        bool TransactionAutoCloseConnection { get; set; }
    }
}