using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using ILogger = Serilog.ILogger;

namespace Sunlit.PointToPoint.Api.Middelwares
{
    /// <summary>
    /// Log 中介層
    /// 將系統上所有的 Log 統一在這個物件上處理。
    /// </summary>
    public class LogMiddleware
    {
        private readonly ILogger _logger;
        private readonly RequestDelegate _next;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        public LogMiddleware(RequestDelegate next)
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
            context.Request.EnableBuffering();
            byte[] buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
            await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
            string requestBody = Encoding.UTF8.GetString(buffer);
            context.Request.Body.Seek(0, SeekOrigin.Begin);

            StringBuilder builder = new StringBuilder(Environment.NewLine);
            List<string> skipUrl = new List<string> { "SIGN-IN" };

            bool isSkipBody = skipUrl.Any(x => x == context.Request.Path.Value.Split('/').Last().ToUpper());

            

            string body =
                $"{context.Request.Scheme} {context.Request.Host}{context.Request.Path} {context.Request.QueryString}";
            if (!isSkipBody) body += $" Request body:{requestBody}";
            builder.AppendLine(body);

            // signalR 略過

            Stream originalBodyStream = context.Response.Body;
            using (MemoryStream responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;
                await _next(context);
                string response = await FormatResponse(context.Response);
                builder.AppendLine($"Response: {response}");
                _logger.Information(builder.ToString());
                await responseBody.CopyToAsync(originalBodyStream);
            }


        }

        /// <summary>
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            string text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return $"{response.StatusCode} {text}";
        }
    }
}
