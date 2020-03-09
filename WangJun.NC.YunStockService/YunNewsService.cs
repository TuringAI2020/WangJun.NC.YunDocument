using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using WangJun.NC.YunStockService.Models;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunStockService
{
    public class YunNewsService
    {
        public static YunNewsService GetInst()
        {
            var inst = new YunNewsService();
            return inst;
        }

        public RES QueryList(string query)
        {
            try
            {
                var setName = "Stock:ShortNews";
                var res  = REDIS.Current.SortedSetQuery(setName, 202003090000, 202003099999, true, 0, 10);
                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

     }
}
