using Interface.Service.Token;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Model.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace Sunlit.Dispatch.Api.Controllers
{
    /// <summary>
    /// Token API
    /// </summary>
    [Route("api/Token")]
    [ApiController]
    public class AccountTokenController: BaseApiController
    {

        private readonly ITokenService _tokenService;
        public AccountTokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;   
        }


        /// <summary>
        /// 取得Token
        /// </summary>
        /// <param name="account">帳號</param>
        /// <returns></returns>
        [HttpGet]
        [Route("CreateToken")]
        [SwaggerResponse(200, Description = "成功", Type = typeof(ResponseModel<string>))]
        [SwaggerResponse(400, Description = "失敗", Type = typeof(ResponseModel<string>))]
        public async Task<IActionResult> CreateToken (string account)
        {
            
        var tmp = await _tokenService.CreateToken(account);
            var result = new ResponseModel<string>()
            {
                statusCode = 200,
                data = tmp
            };
            return Ok(result);
        }
    }
}
