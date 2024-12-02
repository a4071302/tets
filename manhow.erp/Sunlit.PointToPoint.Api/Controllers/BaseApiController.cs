using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Sunlit.Dispatch.Api.Controllers
{
    /// <summary>
    /// API 基礎模型
    /// </summary>
    public class BaseApiController : ControllerBase
    {
        /// <summary>
        /// 取得使用者ID
        /// </summary>
        /// <returns></returns>
        protected string GetUserId()
        {
            if (this.User == null)
            {
                return string.Empty;
            }

            if (this.User.Claims.Count() != 0)
            {
                return this.User.Claims.FirstOrDefault(x =>
                {
                    return x.Type == JwtRegisteredClaimNames.Jti;
                }).Value;
            }
            else
            {
                return string.Empty;
            }
        }

    }
}
