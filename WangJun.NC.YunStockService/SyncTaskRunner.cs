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

        #region 同步概念名称
        protected void SyncConception()
        {
            var setName = "Stock:BaseData:AllConception";
            REDIS.Current.SetTraverse(setName, "*", (setName, item, count, index) =>
            {
                Console.WriteLine($"{item} {index} {count}"); 
                BackDB.Current.Save<Conception>(new Conception { Name=item }, null, new object[] { item });
                return true;
            });
        }
        #endregion

        #region 同步核心题材
        protected void SyncHXTC()
        {
            var dictName_AllCode= "Stock:BaseData:AllCode";

            REDIS.Current.DictTraverse(dictName_AllCode, "*", (dictName1, code, name, count1, index1) =>
            {
                var dictName_Detail = $"Stock:Detail:{code}";
                var res = REDIS.Current.DictTraverse(dictName_Detail,"*", (dictName2, key, value, count2, index2) => {
                    var id = GUID.FromStringToGuid(MD5.ToMD5($"{code}{key}"));
                    BackDB.Current.Save<Hxtc>(new Hxtc { Id= id,Code=code , Title=key,Detail=value,CreateTime=DateTime.Now }, null, new object[] { id });
                    Console.WriteLine($"{code} {key} {value} {index2} {count2}");
                    return true;
                }); 
                return true;
            });
        }
        #endregion

        #region 同步股票和概念关系
        protected void SyncRelationConception()
        {
            var dictName_AllCode = "Stock:BaseData:AllCode";

            //REDIS.Current.DictTraverse(dictName_AllCode, "*", (dictName1, code, name, count1, index1) =>
            //{
            //    var dictName_Detail = $"Stock:Detail:{code}";
            //    var res = REDIS.Current.DictTraverse(dictName_Detail, "*", (dictName2, key, value, count2, index2) => {
            //        var id = GUID.FromStringToGuid(MD5.ToMD5($"{code}{key}"));
            //        BackDB.Current.Save<Hxtc>(new Hxtc { Id = id, Code = code, Title = key, Detail = value, CreateTime = DateTime.Now }, null, new object[] { id });
            //        Console.WriteLine($"{code} {key} {value} {index2} {count2}");
            //        return true;
            //    });
            //    return true;
            //});
        }
        #endregion

        public void Run() {
            //this.SyncStockCode();
            //this.SyncConception();
            this.SyncHXTC();
        }
    }
}
