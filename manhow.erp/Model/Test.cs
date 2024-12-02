using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{

    /// <summary>
    /// 新增據點資料
    /// </summary>
    public class Test
    {
        /// <summary>
        /// 車廠代碼
        /// </summary>
        public string custMfId { get; set; }
        /// <summary>
        /// 經銷商代碼
        /// </summary>
        public string custId { get; set; }
        /// <summary>
        /// 據點代碼
        /// </summary>
        public string? siteId { get; set; }
        /// <summary>
        /// 據點名稱
        /// </summary>
        public string siteName { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string addr { get; set; }
        /// <summary>
        /// 經度
        /// </summary>
        public double lng { get; set; }
        /// <summary>
        /// 緯度
        /// </summary>
        public double lat { get; set; }
        /// <summary>
        /// 有效半徑(公尺)
        /// </summary>
        public int radius { get; set; }
    }
}
