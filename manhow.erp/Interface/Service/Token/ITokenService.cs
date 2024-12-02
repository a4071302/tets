using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interface.Service.Token
{
    public interface ITokenService
    {
        /// <summary>
        /// 建立登入用的Token
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns></returns>
        public Task<string> CreateToken(string account);
    }
}
