using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Response
{
    /// <summary>
    /// 選單資料
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MenuModel<T>
    {
        /// <summary>
        /// 選單Id
        /// </summary>
        public T key {  get; set; }
        /// <summary>
        /// 顯示文字
        /// </summary>
        public string value { get; set; }
    }
}
