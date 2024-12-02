using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace Utility.Helper
{
    public class JwtHelper
    {
        private static Dictionary<string, string> _tokenDic;//站存token記憶體


        public JwtHelper()
        {
            if(_tokenDic == null) _tokenDic = new Dictionary<string, string>();
          
        }

        /// <summary>
        /// 產生JWT Token字串
        /// </summary>
        /// <param name="tokenModel"></param>
        /// <param name="expTime">過期時間</param>
        /// <returns></returns>
        public string IssueJWT(TokenModelJWT tokenModel)
        {
            var dateTime = DateTime.UtcNow;

            string iss = Appsettings.app(new string[] { "Audience", "Issuer" });
            string aud = Appsettings.app(new string[] { "Audience", "Audience" });
            string secret = Appsettings.app(new string[] { "Audience", "Secret" });
            var claims = new List<Claim>
            {
                //下面為Claim標準配置
                new Claim(JwtRegisteredClaimNames.Jti, tokenModel.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}") ,
                //過期時間，目前為30min，注意JWT有自己的緩衝過期時間
                new Claim (JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(tokenModel.ExpTime).ToUnixTimeSeconds()}"),
                new Claim(JwtRegisteredClaimNames.Iss, iss),
                new Claim(JwtRegisteredClaimNames.Aud, aud),
              
            };

            // 可以將一個使用者賦予多个角色；
            if (tokenModel.Role != null)
            {
                claims.AddRange(tokenModel.Role.Split(',').Select(s => new Claim(ClaimTypes.Role, s)));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: iss,
                claims: claims,
                signingCredentials: creds);

            var jwtHandler = new JwtSecurityTokenHandler();
            var encodedJwt = jwtHandler.WriteToken(jwt);

            return encodedJwt;
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="jwtStr"></param>
        /// <returns></returns>
        public TokenModelJWT SerializeJWT(string jwtStr)
        {
            try
            {
                var result = new TokenModelJWT();
                object role = new object();
                // year是系統的年度
                object year = new object();
                object systemId = new object();
                var jwtHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(jwtStr);
                jwtToken.Payload.TryGetValue(ClaimTypes.Role, out role);
              
                result.UserId = jwtToken.Id;
                
                string roleStr = role.ToString();
                //解決多角色問題
                List<string> roleList = new List<string>();
                if (roleStr.Contains("["))
                {
                    roleList = JsonConvert.DeserializeObject<List<string>>(roleStr);
                }
                else
                {
                    roleList.Add(roleStr);
                }
                result.Role = string.Join(",", roleList.Select(r => r).ToArray());// 加入多種身份
                result.ExpTime = jwtToken.ValidTo.ToLocalTime();
                return result;

            }
            catch (Exception ex)
            {
                return new TokenModelJWT();
                //throw new Exception(ex.Message,ex);
            }
        }

        /// <summary>
        /// 將使用者編號與令牌進行儲存或更新至專案記憶體中
        /// </summary>
        /// <param name="userId">使用者</param>
        /// <param name="token"></param>
        public void SetTokenDic(string userId, string token)
        {
            if (_tokenDic.ContainsKey(userId))
            {
                _tokenDic[userId] = token;
            }
            else
            {
                _tokenDic.Add(userId, token);
            }
        }

        /// <summary>
        /// 依使用者編號從專案記憶體中取得token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> getTokenDicByUserId(string userId)
        {
            if (_tokenDic != null && _tokenDic.ContainsKey(userId))
                return _tokenDic[userId];
            else
                return "";
        }

        /// <summary>
        /// 確定記憶體有沒有東西
        /// </summary>
        /// <returns></returns>
        public bool TokenDicExist()
        {
            return _tokenDic.Count > 0;
        }

        /// <summary>
        /// 設定Token
        /// </summary>
        /// <param name="tokenDic"></param>
        /// <returns></returns>
        public async Task<bool> SetTokenDic (Dictionary<string, string> tokenDic)
        {
            _tokenDic = tokenDic;
            return true;
        }

    }

    /// <summary>
    /// Token Info
    /// </summary>
    public class TokenModelJWT
    {
        /// <summary>
        /// 使用者編號
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// 過期時間
        /// </summary>
        public DateTime ExpTime { get; set; }

    }
}
