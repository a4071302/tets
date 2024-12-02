using Interface.Repository.MsSql.ManhowErp;
using Microsoft.Extensions.Caching.Distributed;
using Model.Response;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Utility.Helper;

namespace Sunlit.Dispatch.Api.Middlewares
{
    /// <summary>
    /// 驗證有效的 token 
    /// </summary>
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        //private readonly IDistributedCache _cache;
        private readonly JwtHelper _jwt;
        private readonly IServiceProvider _serviceProvider;
        public JwtMiddleware(RequestDelegate next
            
            , JwtHelper jwt
            , IServiceProvider serviceProvider)
        {
            _next = next;
            //_cache = cache;
            _jwt = jwt;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="claimService"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            Tuple<bool, string> ValidateResult = new Tuple<bool, string>(false, string.Empty);
            string token = context.Request.Headers["Authorization"];
            bool isAllowAnonymousRequest = IsAllowAnonymousRequest(context.Request.Path.Value.ToLower());

            if (token != null && !isAllowAnonymousRequest)
            {
                token = token.Replace("Bearer ", "");
                TokenModelJWT tokenObj = _jwt.SerializeJWT(token);
                ValidateResult = await ValidateToken(tokenObj, token);
                if (ValidateResult.Item1)
                {
                    await _next(context);
                }
                else
                {
                    string responseBody = JsonConvert.SerializeObject(new ResponseModel<string>()
                    {
                        statusCode = StatusCodes.Status403Forbidden,
                        message = ValidateResult.Item2
                    });

                    context.Response.StatusCode = StatusCodes.Status200OK;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(responseBody);
                }
            }
            else if (!isAllowAnonymousRequest && token == null)
            {
                string responseBody = JsonConvert.SerializeObject(new ResponseModel<string>()
                {
                    statusCode = StatusCodes.Status403Forbidden,
                    message = "請登入後再試"
                });

                context.Response.StatusCode = StatusCodes.Status200OK;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(responseBody);
            }
            else if (isAllowAnonymousRequest)
            {
                await _next(context);
            }

        }

        private async Task< Tuple<bool, string>>ValidateToken(TokenModelJWT tokenObj, string reqestToken)
        {
            bool status = false;
            string message = string.Empty;

            if(!_jwt.TokenDicExist()) // 如果記憶體沒有的話，去資料庫撈
            {
                using var scope = _serviceProvider.CreateScope();
                {
                    var tokenRepository = scope.ServiceProvider.GetRequiredService<ITokenRepository>();
                    var allToken = await tokenRepository.GetAll();
                    var tokenDic = allToken.ToDictionary(x => x.UserId, x => x.TokenContent);
                    await _jwt.SetTokenDic(tokenDic);
                }
            }

            string token =await  _jwt.getTokenDicByUserId(tokenObj.UserId);

            if (string.IsNullOrEmpty(token))
            {
                message = "該帳號尚未登入";
            } 
            else if(token != reqestToken)
            {
                message = "請重新登入";
            }
            else if(token == reqestToken)
            {
                status = true;
            }
            return Tuple.Create(status, message);
        }

        /// <summary>
        /// 判斷是否為允許匿名的請求(不用token)
        /// </summary>
        /// <param name="path">請求路徑</param>
        private bool IsAllowAnonymousRequest(string path)
        {
            //actionName全部都要小寫
            List<string> filterPath = new List<string> { "swagger", "favicon.ico", "error" };
            //account
            filterPath.AddRange(new List<string>() { "createtoken", "login", "logout", "refreshtoken", "verifyemail", "authorizetest",  });
            // signalR
            filterPath.AddRange(new List<string>() { "chathub", "sendmessagetoall", "sendmessagebymaincompanyid", "sendmessagebyuserid" });
           
            
            filterPath.AddRange(new List<string>() { "getmemoimagebypath" });

            int actionIndex = 1;
            string actionName = string.Empty;
            if (path.Substring(path.Length - 1, 1) == "/")
            {
                actionIndex = 2;
            }
            actionName = path.Split('/')[path.Split('/').Length - actionIndex];
            string swaggerName = path.Split('/')[1];

            return filterPath.Any(p => p == actionName || p == swaggerName);
        }

    }
}
