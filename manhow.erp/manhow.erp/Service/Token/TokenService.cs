using Interface.Repository.MsSql.ManhowErp;
using Interface.Service.Token;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Helper;

namespace Service.Token
{
    public class TokenService: ITokenService
    {
        private readonly int _tokenExp = 0;
        private readonly int _refreshExp = 0;
        private readonly JwtHelper _jwt; 
        private readonly IConfiguration _configuration;
        private readonly ITokenRepository _tokenRepository;


        public TokenService(JwtHelper jwt
            , IConfiguration configuration
            , ITokenRepository tokenRepository)
        {
            _jwt = jwt;
            _configuration = configuration;
            _tokenExp = Convert.ToInt32(configuration.GetSection("Audience")["TokenExp"]);
            _refreshExp = Convert.ToInt32(configuration.GetSection("Audience")["RefreshExp"]);
            _tokenRepository = tokenRepository;
        }

        public async Task<string> CreateToken(string account)
        {
            Tuple<string, string> tokenStr = this.GetAccessAndRefresh(account, "Admin");
            _jwt.SetTokenDic(account, tokenStr.Item1);
            await _tokenRepository.DeleteByUserId(account);
            var insertData = new Model.ManhowErpTable.Token();
            insertData.CreateUser = "CreateToken";
            insertData.CreateTime = DateTime.Now;
            insertData.TokenContent = tokenStr.Item1;
            insertData.UserId = account;
            insertData.ExpiredTime = DateTime.Now;
            await _tokenRepository.Insert(insertData);
            return await Task.Run(() => tokenStr.Item1);
        }


        /// <summary>
        /// 取得登入與刷新用token
        /// </summary>
        /// <param name="userId">使用者帳號</param>
        /// <param name="role">角色</param>
        /// <param name="systemId">系統編號</param>
        /// <returns></returns>
        private Tuple<string, string> GetAccessAndRefresh(string userId, string role)
        {
            List<int> expTimeList = new List<int>() { _tokenExp, _refreshExp }; //過期時間設定(min)
            string[] tokenArrary = new string[2];
            TokenModelJWT token = new TokenModelJWT();
            foreach (int exp in expTimeList)
            {
                token.UserId = userId;
                token.Role = role;
                token.ExpTime = DateTime.Now.AddMinutes(exp);
                tokenArrary[expTimeList.IndexOf(exp)] = _jwt.IssueJWT(token);
            }
            return new Tuple<string, string>(tokenArrary[0], tokenArrary[1]);
        }



    }
}
