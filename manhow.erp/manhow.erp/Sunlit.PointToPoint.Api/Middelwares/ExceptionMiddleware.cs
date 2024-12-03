using Model.Response;
using Newtonsoft.Json;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ILogger = Serilog.ILogger;

namespace Sunlit.PointToPoint.Api.Middelwares
{

    /// <summary>
    /// Exception 中介層，
    /// 將系統上所有的 Exception 統一在這個物件上處理。
    /// </summary>
    public class ExceptionMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
            _logger = Log.Logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                byte[] buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
                await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
                string requestBody = Encoding.UTF8.GetString(buffer);


                // 記錄錯誤內容

                string request = await FormatRequest(context.Request); // 解析require資料內容轉成文字
                _logger.Error($"{request} , Message: {JsonConvert.SerializeObject(ex)}, StackTrace: {ex.StackTrace}");
                context.Response.ContentType = "application/json";
                string responseBody = JsonConvert.SerializeObject(new ResponseModel<string>()
                {
                    statusCode = 400,
                    message = ex.Message
                });

                await context.Response.WriteAsync(responseBody);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<string> FormatRequest(HttpRequest request)
        {
            Stream body = request.Body;
            byte[] buffer = new byte[Convert.ToInt32(request.ContentLength)];
            await request.Body.ReadAsync(buffer, 0, buffer.Length);
            string bodyAsText = Encoding.UTF8.GetString(buffer);
            request.Body = body;
            return $"{request.Scheme} {request.Host}{request.Path} {request.QueryString} {bodyAsText} \n {request}";
        }
    }
}
