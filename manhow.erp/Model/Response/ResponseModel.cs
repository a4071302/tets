using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Response
{
    /// <summary>
    /// API回應資料
    /// </summary>
    public class ResponseModel<T>
    {
        /// <summary>
        /// 狀態碼
        /// </summary>
        public int statusCode { get; set; }

        /// <summary>
        /// 錯誤訊息
        /// </summary>
        public string message { get; set; } = string.Empty;

        public T data { get; set; }

    }
}
