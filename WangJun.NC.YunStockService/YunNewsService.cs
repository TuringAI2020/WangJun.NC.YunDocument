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
                var startDateTag = long.Parse(DateTime.Now.Date.ToString("yyyyMMdd0000"));
                var endDateTag = long.Parse(DateTime.Now.Date.ToString("yyyyMMdd9999"));
                var res  = REDIS.Current.SortedSetQuery(setName, startDateTag, endDateTag, true, 0, 10);
                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

     }
}
