using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Require
{
    /// <summary>
    /// base64 檔案
    /// </summary>
    public class Base64FileModel
    {
        /// <summary>
        /// 檔案名稱
        /// </summary>
        public string fileName { get; set; }

        /// <summary>
        /// base64字串
        /// </summary>
        public string base64String { get; set; }
    }
}
