using System;
using WangJun.NC.YunStockService.Models;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunStockService
{
    /// <summary>
    /// 同步工具
    /// </summary>
    public class SyncTaskRunner
    {
        #region 同步股票代码
        protected void SyncStockCode()
        {
            var dictName = "Stock:BaseData:AllCode";
            REDIS.Current.DictTraverse(dictName, "*", (dictName, key, value, count, index) =>
            {
                Console.WriteLine($"{key} {value} {index} {count}");
                var prefix = "SZ";
                if (key.StartsWith("6"))
                {
                    prefix = "SH";
                }
                BackDB.Current.Save<StockCode>(new StockCode { Code = key, Name = value,Prefix=prefix } , null,new object[] {key });
                return true;
            });
        }
        #endregion

        public void Run() {
            this.SyncStockCode();
        }
    }
}
