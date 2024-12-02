using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.ManhowErpTable
{
    public class Token
    {
        /// <summary>
        /// 系統編號
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 使用者編號
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 令牌內容
        /// </summary>
        public string TokenContent { get; set; }

        /// <summary>
        /// 建立人員
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// token 過期時間
        /// </summary>
        public DateTime ExpiredTime { get; set; }
    }
}
