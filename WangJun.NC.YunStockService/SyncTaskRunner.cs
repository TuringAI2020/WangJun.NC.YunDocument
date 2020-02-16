using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
                BackDB.Current.Save<StockCode>(new StockCode { Code = key, Name = value, Prefix = prefix }, null, new object[] { key });
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
                BackDB.Current.Save<Conception>(new Conception { Name = item }, null, new object[] { item });
                return true;
            });
        }
        #endregion

        #region 同步核心题材
        protected void SyncHXTC()
        {
            var dictName_AllCode = "Stock:BaseData:AllCode";

            REDIS.Current.DictTraverse(dictName_AllCode, "*", (dictName1, code, name, count1, index1) =>
            {
                var dictName_Detail = $"Stock:Detail:{code}";
                var res = REDIS.Current.DictTraverse(dictName_Detail, "*", (dictName2, key, value, count2, index2) =>
                {
                    var id = GUID.FromStringToGuid(MD5.ToMD5($"{code}{key}"));
                    BackDB.Current.Save<Hxtc>(new Hxtc { Id = id, Code = code, Title = key, Detail = value, CreateTime = DateTime.Now }, null, new object[] { id });
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
            var res = REDIS.Current.QueryKeys("Stock:Relation:Conception:*");
            if (res.SUCCESS)
            {
                var keys = res.DATA as List<string>;
                keys.ForEach(p =>
                {
                    var res = REDIS.Current.SortedSetTraverse(p, "*", (setName, element, score, count, index) =>
                    {
                        var keyArr = setName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        var conception = keyArr[keyArr.Length - 1];
                        BackDB.Current.Save<RelationConception>(new RelationConception { Code = element, Conception = conception, Score = Convert.ToDecimal(score) }, null, new object[] { element, conception });
                        Console.WriteLine($"{setName} {element} {score} {index} {count}");
                        return true;
                    });

                });
            }
        }
        #endregion

        #region 同步财务主要指标
        protected void Sync财务主要指标()
        {
            var dictName_AllCode = "Stock:BaseData:AllCode";

            REDIS.Current.DictTraverse(dictName_AllCode, "*", (dictName1, code, name, count1, index1) =>
            {
                var dictName_Detail = $"Stock:Detail:{code}";
                var res = REDIS.Current.DictTraverse(dictName_Detail, "财务主要指标:*", (dictName2, key, jsonStr, count2, index2) =>
                {
                    jsonStr = jsonStr.Replace("(天)", string.Empty).Replace("(元)", string.Empty).Replace("(%)", string.Empty).Replace("/", string.Empty);
                    var jsonData = JSON.ToObject<财务主要指标>(jsonStr);
                    //var id = GUID.FromStringToGuid(MD5.ToMD5($"{code}{key}"));
                    BackDB.Current.Save<财务主要指标>(jsonData, null, new object[] { jsonData.Code, jsonData.DateTag });
                    Console.WriteLine($"{code} {key} {jsonStr} {index2} {count2}");
                    return true;
                });
                return true;
            });
        }
        #endregion

        #region 同步北向成交明细
        protected Dictionary<int, Thread> threads北向成交明细 = new Dictionary<int, Thread>();
        protected void Sync北向成交明细()
        {
            var dictName_AllCode = "Stock:BaseData:AllCode";

            REDIS.Current.DictTraverse(dictName_AllCode, "*", (dictName1, code, name, count1, index1) =>
            {
                Task.Run(() =>
                {
                    var db = BackDB.New;
                    Console.WriteLine($"开始  {this.threads北向成交明细.Count} {code} ");
                    lock (this.threads北向成交明细)
                    {
                        if (!this.threads北向成交明细.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        {
                            this.threads北向成交明细.Add(Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread);
                        }
                    }
                    var dictName_Detail = $"Stock:BXCJMX:{code}";
                    var res = REDIS.Current.SortedSetTraverse(dictName_Detail, "*", (dictName2, jsonStr, score, count2, index2) =>
                    {
                        var jsonData = JSON.ToObject<北向成交明细>(jsonStr);
                        //var id = GUID.FromStringToGuid(MD5.ToMD5($"{code}{key}"));
                        db.Save<北向成交明细>(jsonData, null, new object[] { jsonData.Code, jsonData.日期tag });
                        Console.WriteLine($"{this.threads北向成交明细.Count} {code} {score} {jsonStr} {index2} {count2}");
                        return true;
                    });

                    lock (this.threads北向成交明细)
                    {
                        if (this.threads北向成交明细.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        {
                            this.threads北向成交明细.Remove(Thread.CurrentThread.ManagedThreadId);
                        }
                    }
                    Console.WriteLine($"结束  {this.threads北向成交明细.Count} {code} ");
                });
                Console.WriteLine($"等待秒数  {this.threads北向成交明细.Count} {code} ");
                Thread.Sleep( 1000 * this.threads北向成交明细.Count);
                return true;
            });
        }
        #endregion

        #region 同步北向持股明细
        protected Dictionary<int, Thread> threads北向持股明细 = new Dictionary<int, Thread>();
        protected void Sync北向持股明细()
        {
            var dictName_AllCode = "Stock:BaseData:AllCode";

            REDIS.Current.DictTraverse(dictName_AllCode, "*", (dictName1, code, name, count1, index1) =>
            {
                Task.Run(() =>
                {
                    var db = BackDB.New;
                    Console.WriteLine($"开始  {this.threads北向持股明细.Count} {code} ");
                    lock (this.threads北向持股明细)
                    {
                        if (!this.threads北向持股明细.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        {
                            this.threads北向持股明细.Add(Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread);
                        }
                    }
                    var dictName_Detail = $"Stock:北向持股:{code}";
                    var res = REDIS.Current.SortedSetTraverse(dictName_Detail, "*", (dictName2, jsonStr, score, count2, index2) =>
                    {
                        var jsonData = JSON.ToObject<北向持股明细>(jsonStr);
                        jsonData.Code = code;
                        db.Save<北向持股明细>(jsonData, null, new object[] { jsonData.Code, jsonData.持股日期tag,jsonData.机构名称 });
                        Console.WriteLine($"{this.threads北向持股明细.Count} {code} {score} {jsonStr} {index2} {count2}");
                        return true;
                    });

                    lock (this.threads北向持股明细)
                    {
                        if (this.threads北向持股明细.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        {
                            this.threads北向持股明细.Remove(Thread.CurrentThread.ManagedThreadId);
                        }
                    }
                    Console.WriteLine($"结束  {this.threads北向持股明细.Count} {code} ");
                });
                Console.WriteLine($"等待秒数  {this.threads北向持股明细.Count} {code} ");
                Thread.Sleep(1000 * this.threads北向持股明细.Count);
                return true;
            });
        }
        #endregion

        #region 同步融资融券
        protected Dictionary<int, Thread> threads融资融券 = new Dictionary<int, Thread>();
        protected void Sync融资融券()
        {
            var dictName_AllCode = "Stock:BaseData:AllCode";

            REDIS.Current.DictTraverse(dictName_AllCode, "*", (dictName1, code, name, count1, index1) =>
            {
                Task.Run(() =>
                {
                    var db = BackDB.New;
                    Console.WriteLine($"开始  {this.threads融资融券.Count} {code} ");
                    lock (this.threads融资融券)
                    {
                        if (!this.threads融资融券.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        {
                            this.threads融资融券.Add(Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread);
                        }
                    }
                    var dictName_Detail = $"Stock:RZRQ:{code}";
                    var res = REDIS.Current.SortedSetTraverse(dictName_Detail, "*", (dictName2, jsonStr, score, count2, index2) =>
                    {
                        try
                        {
                            var jsonData = JSON.ToObject<融资融券>(jsonStr);
                            db.Save<融资融券>(jsonData, null, new object[] { jsonData.Code, jsonData.交易日期tag });
                            Console.WriteLine($"融资融券 {this.threads融资融券.Count} {code} {score} {jsonStr} {index2} {count2}");
                            return true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"融资融券 {e.Message}");
                            Console.Beep(14000, 3000);
                            return true;
                        } 
                    });

                    lock (this.threads融资融券)
                    {
                        if (this.threads融资融券.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        {
                            this.threads融资融券.Remove(Thread.CurrentThread.ManagedThreadId);
                        }
                    }
                    Console.WriteLine($"结束  {this.threads融资融券.Count} {code} ");
                });
                Console.WriteLine($"等待秒数  {this.threads融资融券.Count} {code} ");
                Thread.Sleep(1000 * this.threads融资融券.Count);
                return true;
            });
        }
        #endregion

        #region 同步资金流向
        protected Dictionary<int, Thread> threads资金流向 = new Dictionary<int, Thread>();
        protected void Sync资金流向()
        {
            var dictName_AllCode = "Stock:BaseData:AllCode";

            REDIS.Current.DictTraverse(dictName_AllCode, "*", (dictName1, code, name, count1, index1) =>
            {
                Task.Run(() =>
                {
                    var db = BackDB.New;
                    Console.WriteLine($"开始  {this.threads资金流向.Count} {code} ");
                    lock (this.threads资金流向)
                    {
                        if (!this.threads资金流向.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        {
                            this.threads资金流向.Add(Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread);
                        }
                    }
                    var dictName_Detail = $"Stock:ZJLX:{code}";
                    var res = REDIS.Current.SortedSetTraverse(dictName_Detail, "*", (dictName2, jsonStr, score, count2, index2) =>
                    {
                        try
                        {
                            var jsonData = JSON.ToObject<资金流向>(jsonStr);
                            db.Save<资金流向>(jsonData, null, new object[] { jsonData.Code, jsonData.日期tag });
                            Console.WriteLine($"资金流向 {this.threads资金流向.Count} {code} {score} {jsonStr} {index2} {count2}");
                            return true;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine($"资金流向 {e.Message}");
                            Console.Beep(14000, 3000);
                            return true;
                        }
                    });

                    lock (this.threads资金流向)
                    {
                        if (this.threads资金流向.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        {
                            this.threads资金流向.Remove(Thread.CurrentThread.ManagedThreadId);
                        }
                    }
                    Console.WriteLine($"结束  {this.threads资金流向.Count} {code} ");
                });
                Console.WriteLine($"等待秒数  {this.threads资金流向.Count} {code} ");
                Thread.Sleep(1000 * this.threads资金流向.Count);
                return true;
            });
        }

        #endregion

        #region 反向同步重要代码到REDIS
        protected void 反向同步重要代码()
        {
            var db = BackDB.New;
            var list = db.QueryList<重要代码>(null).ToList();
            list.ForEach(p=> {
                REDIS.Current.SortedSetAdd("Stock:Task:VIPCode", p.Code, 0);
                Console.WriteLine(p.Code);
            });
        }
        #endregion

        public void Run()
        {
            //this.SyncStockCode();
            //this.SyncConception();
            //this.SyncHXTC();
            //this.SyncRelationConception();
            //this.Sync财务主要指标();
            //this.Sync北向成交明细();
            //this.Sync北向持股明细();
            //this.Sync融资融券();
            //this.Sync资金流向();
            this.反向同步重要代码();
        }
    }
}
