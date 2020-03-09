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
        protected Dictionary<int, Thread> threads财务主要指标 = new Dictionary<int, Thread>();
        protected void Sync财务主要指标()
        {
            var dictName_AllCode = "Stock:BaseData:AllCode";

            REDIS.Current.DictTraverse(dictName_AllCode, "*", (dictName1, code, name, count1, index1) =>
            {
                Task.Run(() =>
                {
                    var db = BackDB.New;
                    Console.WriteLine($"开始  {this.threads财务主要指标.Count} {code} ");
                    lock (this.threads财务主要指标)
                    {
                        if (!this.threads财务主要指标.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        {
                            this.threads财务主要指标.Add(Thread.CurrentThread.ManagedThreadId, Thread.CurrentThread);
                        }
                    }
                    var dictName_Detail = $"Stock:Detail:{code}";
                    var res = REDIS.Current.DictTraverse(dictName_Detail, "财务主要指标:*", (dictName2, key, jsonStr, count2, index2) =>
                    {
                        jsonStr = jsonStr.Replace("(天)", string.Empty).Replace("(元)", string.Empty).Replace("(%)", string.Empty).Replace("/", string.Empty);
                        var jsonData = JSON.ToObject<财务主要指标>(jsonStr);
                        //var id = GUID.FromStringToGuid(MD5.ToMD5($"{code}{key}"));
                        db.Save<财务主要指标>(jsonData, null, new object[] { jsonData.Code, jsonData.DateTag });
                        Console.WriteLine($"{code} {key} {jsonStr} {index2} {count2}");
                        return true;
                    });

                    lock (this.threads财务主要指标)
                    {
                        if (this.threads财务主要指标.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                        {
                            this.threads财务主要指标.Remove(Thread.CurrentThread.ManagedThreadId);
                        }
                    }
                    Console.WriteLine($"结束  {this.threads财务主要指标.Count} {code} ");
                });
                Console.WriteLine($"等待秒数  {this.threads财务主要指标.Count} {code} ");
                Thread.Sleep(1000 * this.threads财务主要指标.Count);
                return true;
            });
        }
        #endregion

        #region 增量财务主要指标 
        protected void IncSyncCWFX(string qName)
        {
            var db = BackDB.New;
            var qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            do
            {
                if (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>)
                {
                    try
                    { 
                        object qData = qRes.DATA;
                        if (qData is Dictionary<string, object> && (qData as Dictionary<string, object>).ContainsKey("value"))
                        {
                            qData = (qData as Dictionary<string, object>)["value"];
                        }
                        var jsonStr = JSON.ToJson(qData);
                        jsonStr = jsonStr.Replace("(天)", string.Empty).Replace("(元)", string.Empty).Replace("(%)", string.Empty).Replace("/", string.Empty).Replace("(次)", string.Empty);
                        var jsonData = JSON.ToObject<财务主要指标>(jsonStr);
                        var dbRes = db.Save<财务主要指标>(jsonData, null, new object[] { jsonData.Code, jsonData.DateTag });
                        Console.WriteLine($"增量财务主要指标 {dbRes} {jsonStr}");
                    }
                    catch (Exception e)
                    {
                        REDIS.Current.Enqueue(qName, qRes.DATA);
                        Console.WriteLine($"增量财务主要指标异常 {e.Message}");
                    }
                }
                qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            }
            while (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>);
        }
        #endregion

        #region 增量同步北向成交 
        protected void IncSyncBXCJ(string qName)
        {
            var db = BackDB.New;
            var qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            do
            {
                if (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>)
                {
                    try
                    {  
                        object qData = qRes.DATA;
                        if (qData is Dictionary<string, object> && (qData as Dictionary<string, object>).ContainsKey("value"))
                        {
                            qData = (qData as Dictionary<string, object>)["value"];
                        }
                        var jsonStr = JSON.ToJson(qData); 
                        var jsonData = JSON.ToObject<北向成交明细>(jsonStr);
                        var dbRes = db.Save<北向成交明细>(jsonData, null, new object[] { jsonData.Code, jsonData.日期tag });
                        Console.WriteLine($"增量同步北向成交 {dbRes} {jsonStr}");
 
                    }
                    catch (Exception e)
                    {
                        REDIS.Current.Enqueue(qName, qRes.DATA);
                        Console.WriteLine($"增量同步北向成交 异常 {e.Message}");
                    }
                }
                qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            }
            while (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>);
        }
        #endregion

        #region 增量同步北向持股
        protected void IncSyncBXCG(string qName)
        {
            var db = BackDB.New;
            var qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            do
            {
                if (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>)
                {
                    try
                    {
                        //var qScore = int.Parse((qRes.DATA as Dictionary<string, object>)["score"].ToString());
                        object qData = qRes.DATA ;
                        if (qData is Dictionary<string, object>  && (qData as Dictionary<string, object>).ContainsKey("value"))
                        {
                            qData = (qData as Dictionary<string, object>)["value"];
                        }
                        //var qCode = (qRes.DATA as Dictionary<string, object>)["code"];

                        var jsonStr = JSON.ToJson(qData);
                        var jsonData = JSON.ToObject<北向持股明细>(jsonStr);
                        var dbRes = db.Save<北向持股明细>(jsonData, null, new object[] { jsonData.Code, jsonData.持股日期tag, jsonData.机构名称 });
                        Console.WriteLine($"增量同步北向持股 {dbRes} {jsonStr}");

                    }
                    catch (Exception e)
                    {
                        REDIS.Current.Enqueue(qName, qRes.DATA);
                        Console.WriteLine($"增量同步北向持股 异常 {e.Message} {qName} {JSON.ToJson(qRes)}");
                        Thread.Sleep(10 * 1000);
                    }
                }
                qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            }
            while (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>);
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
                Thread.Sleep(3000 * this.threads北向成交明细.Count);
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
                    var dictName_Detail = $"Stock:BXCGMX:{code}";
                    var res = REDIS.Current.SortedSetTraverse(dictName_Detail, "*", (dictName2, jsonStr, score, count2, index2) =>
                    {
                        var jsonData = JSON.ToObject<北向持股明细>(jsonStr);
                        jsonData.Code = code;
                        db.Save<北向持股明细>(jsonData, null, new object[] { jsonData.Code, jsonData.持股日期tag, jsonData.机构名称 });
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

        #region 增量同步融资融券
        protected void IncSyncRZRQ(string qName)
        {
            var db = BackDB.New;
            var qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            do
            {
                if (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>)
                {
                    try
                    { 
                        object qData = qRes.DATA;
                        if (qData is Dictionary<string, object> && (qData as Dictionary<string, object>).ContainsKey("value"))
                        {
                            qData = (qData as Dictionary<string, object>)["value"];
                        } 

                        var jsonStr = JSON.ToJson(qData);
                        var jsonData = JSON.ToObject<融资融券>(jsonStr);
                        var dbRes = db.Save<融资融券>(jsonData, null, new object[] { jsonData.Code, jsonData.交易日期tag });
                        Console.WriteLine($"增量同步融资融券 {dbRes} {jsonStr}");

                    }
                    catch (Exception e)
                    {
                        REDIS.Current.Enqueue(qName, qRes.DATA);
                        Console.WriteLine($"增量同步融资融券 异常 {e.Message} {qName} {JSON.ToJson(qRes)}");
                        Thread.Sleep(10 * 1000);
                    }
                }
                qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            }
            while (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>);
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

        #region 增量同步资金流向
        protected void IncSyncZJLX(string qName)
        {
            var db = BackDB.New;
            var qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            do
            {
                if (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>)
                {
                    try
                    {
                        object qData = qRes.DATA;
                        if (qData is Dictionary<string, object> && (qData as Dictionary<string, object>).ContainsKey("value"))
                        {
                            qData = (qData as Dictionary<string, object>)["value"];
                        }

                        var jsonStr = JSON.ToJson(qData);
                        var jsonData = JSON.ToObject<资金流向>(jsonStr);
                        var dbRes = db.Save<资金流向>(jsonData, null, new object[] { jsonData.Code, jsonData.日期tag });
                        Console.WriteLine($"增量同步资金流向 {dbRes} {jsonStr}");

                    }
                    catch (Exception e)
                    {
                        REDIS.Current.Enqueue(qName, qRes.DATA);
                        Console.WriteLine($"增量同步资金流向 异常 {e.Message} {qName} {JSON.ToJson(qRes)}");
                        Thread.Sleep(10 * 1000);
                    }
                }
                qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            }
            while (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>);
        }
        #endregion

        #region 反向同步重要代码到REDIS
        protected void 反向同步重要代码()
        {
            var db = BackDB.New;
            var list = db.QueryList<重要代码>(null).ToList();
            var setName = "Stock:Task:VIPCode";
            REDIS.Current.SortedSetRemoveByScore(setName);
            list.ForEach(p =>
            {
                REDIS.Current.SortedSetAdd("Stock:Task:VIPCode", p.Code, p.Sort.HasValue ? p.Sort.Value : 0);
                Console.WriteLine($"反向同步重要代码 {p.Code} {p.Sort}");
            });
        }
        #endregion

        #region 同步北向代码
        protected void 同步北向代码()
        {
            var setName = $"Stock:BXCode";
            var qSyncName = $"Stock:Sync:2DB:{setName}";
            var db = BackDB.New;
            var qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qSyncName);
            do
            {
                if (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>)
                {
                    try
                    {
                        object qData = qRes.DATA; 

                        var jsonStr = JSON.ToJson(qData);
                        var jsonData = JSON.ToObject<北向代码>(jsonStr);
                        var dbRes = db.Save<北向代码>(jsonData, null, new object[] { jsonData.Code});
                        Console.WriteLine($"同步北向代码 {dbRes} {jsonStr}");

                    }
                    catch (Exception e)
                    {
                        REDIS.Current.Enqueue(qSyncName, qRes.DATA);
                        Console.WriteLine($"同步北向代码异常 {e.Message} {qSyncName} {JSON.ToJson(qRes)}");
                        Thread.Sleep(10 * 1000);
                    }
                }
                qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qSyncName);
            }
            while (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>);
        }
        #endregion

        #region 同步所有机构
        protected void 同步所有机构()
        {
            var setName = $"Stock:JG";
            var qSyncName = $"Stock:Sync:2DB:{setName}";
            var db = BackDB.New;
            var qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qSyncName);
            do
            {
                if (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>)
                {
                    try
                    {
                        object qData = qRes.DATA;

                        var jsonStr = JSON.ToJson(qData);
                        var jsonData = JSON.ToObject<所有机构>(jsonStr);
                        var dbRes = db.Save<所有机构>(jsonData, null, new object[] { jsonData.JgCode,jsonData.JgName });
                        Console.WriteLine($"同步所有机构 {dbRes} {jsonStr}");

                    }
                    catch (Exception e)
                    {
                        REDIS.Current.Enqueue(qSyncName, qRes.DATA);
                        Console.WriteLine($"同步所有机构异常 {e.Message} {qSyncName} {JSON.ToJson(qRes)}");
                        Thread.Sleep(10 * 1000);
                    }
                }
                qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qSyncName);
            }
            while (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>);
        }
        #endregion

        #region 增量同步北向持股统计
        protected void IncSyncBXCGTJ(string qName)
        {
            var db = BackDB.New;
            var qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            do
            {
                if (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>)
                {
                    try
                    {
                        object qData = qRes.DATA; 
                        var jsonStr = JSON.ToJson(qData);
                        var jsonData = JSON.ToObject<北向持股统计>(jsonStr);
                        var dbRes = db.Save<北向持股统计>(jsonData, null, new object[] { jsonData.持股日期tag, jsonData.机构名称 });
                        Console.WriteLine($"增量同步北向持股统计 {dbRes} {jsonStr}");

                    }
                    catch (Exception e)
                    {
                        REDIS.Current.Enqueue(qName, qRes.DATA);
                        Console.WriteLine($"增量同步北向持股统计 异常 {e.Message} {qName} {JSON.ToJson(qRes)}");
                        Thread.Sleep(10 * 1000);
                    }
                }
                qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            }
            while (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>);
        }
        #endregion

        #region 增量同步北向机构持股明细 - 机构维度
        protected void IncSyncBXCGMXURL(string qName)
        {
            var db = BackDB.New;
            var qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            do
            {
                if (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>)
                {
                    try
                    {
                        object qData = qRes.DATA;
                        var jsonStr = JSON.ToJson(qData);
                        var jsonData = JSON.ToObject<北向机构持股明细>(jsonStr);
                        jsonData.Id = GUID.FromStringToGuid($"{jsonData.Code}_{jsonData.持股日期tag}_{jsonData.JgCode}");
                        var dbRes = db.Save<北向机构持股明细>(jsonData, null, new object[] { jsonData.Id });
                        Console.WriteLine($"增量同步北向机构持股明细 {dbRes} {jsonStr}");

                    }
                    catch (Exception e)
                    {
                        REDIS.Current.Enqueue(qName, qRes.DATA);
                        Console.WriteLine($"增量同步北向机构持股明细 异常 {e.Message} {qName} {JSON.ToJson(qRes)}");
                        Thread.Sleep(10 * 1000);
                    }
                }
                qRes = REDIS.Current.Dequeue<Dictionary<string, object>>(qName);
            }
            while (qRes.SUCCESS && qRes.DATA is Dictionary<string, object>);
        }
        #endregion

        #region 增量同步东方财富网快讯
        protected void IncSync<T>(string qName,Func<T,object[]> callbackGetKey) where T:class,new()
        {
            var db = BackDB.New;
            var qRes = REDIS.Current.Dequeue<T>(qName);
            do
            {
                if (qRes.SUCCESS && qRes.DATA is T)
                {
                    try
                    {
                        object qData = qRes.DATA;
                        var jsonStr = JSON.ToJson(qData);
                        var jsonData = JSON.ToObject<T>(jsonStr);
                        var keys = callbackGetKey(jsonData);
                        var dbRes = db.Save<T>(jsonData, null, keys);
                        Console.WriteLine($"增量同步 {typeof(T)} {dbRes} {jsonStr}");
                    }
                    catch (Exception e)
                    {
                        REDIS.Current.Enqueue(qName, qRes.DATA);
                        Console.WriteLine($"增量同步  {typeof(T)} 异常 {e.Message} {qName} {JSON.ToJson(qRes)}");
                        Thread.Sleep(10 * 1000);
                    }
                }
                qRes = REDIS.Current.Dequeue<T>(qName);
            }
            while (qRes.SUCCESS && qRes.DATA is T);
        }
        #endregion

        protected Dictionary<string, string> threads增量同步 = new Dictionary<string, string>();
        protected void AutoSync()
        {
            while (true)
            {
                if (false)
                {
                    #region 增量同步财务分析 

                    try
                    {
                        var taskName = "增量同步财务分析";
                        var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:CWFX:*");
                        if (resKeys.SUCCESS)
                        {
                            var keys = resKeys.DATA as List<string>;
                            if (null != keys && 0 < keys.Count)
                            {
                                var startNewTask = true;
                                lock (this.threads增量同步)
                                {
                                    if (!this.threads增量同步.ContainsKey(taskName))
                                    {
                                        this.threads增量同步.Add(taskName, "Running");
                                        startNewTask = true;
                                    }
                                    else
                                    {
                                        startNewTask = false;
                                    }
                                }
                                if (startNewTask)
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            keys.ForEach(p =>
                                            {
                                                this.IncSyncCWFX(p);
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                        }
                                        finally
                                        {
                                            lock (this.threads增量同步)
                                            {
                                                if (this.threads增量同步.ContainsKey(taskName))
                                                {
                                                    this.threads增量同步.Remove(taskName);
                                                }
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"{taskName} 暂时没有要同步的数据");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"增量同步财务分析 检查任务异常 {e.Message}");
                    }
                    #endregion

                    #region 重要代码同步
                    try
                    {
                        var keyName = "Stock:Sync:2Redis:重要代码";
                        var res = REDIS.Current.CacheGet<string>(keyName);
                        if (!res.SUCCESS)
                        {
                            this.反向同步重要代码();
                            REDIS.Current.CacheSet(keyName, DateTime.Now.ToString(), new TimeSpan(3, 0, 0));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"执行反向同步重要代码任务出现异常 {ex.Message}");
                    }
                    #endregion

                    #region 增量同步北向成交 

                    try
                    {
                        var taskName = "增量同步北向成交";
                        var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:BXCJMX:*");
                        if (resKeys.SUCCESS)
                        {
                            var keys = resKeys.DATA as List<string>;
                            if (null != keys && 0 < keys.Count)
                            {
                                var startNewTask = true;
                                lock (this.threads增量同步)
                                {
                                    if (!this.threads增量同步.ContainsKey(taskName))
                                    {
                                        this.threads增量同步.Add(taskName, "Running");
                                        startNewTask = true;
                                    }
                                    else
                                    {
                                        startNewTask = false;
                                    }
                                }
                                if (startNewTask)
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            keys.ForEach(p =>
                                            {
                                                this.IncSyncBXCJ(p);
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                        }
                                        finally
                                        {
                                            lock (this.threads增量同步)
                                            {
                                                if (this.threads增量同步.ContainsKey(taskName))
                                                {
                                                    this.threads增量同步.Remove(taskName);
                                                }
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"{taskName} 暂时没有要同步的数据");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"增量同步北向成交 检查任务异常 {e.Message}");
                    }
                    #endregion

                    #region 增量同步北向持股

                    try
                    {
                        var taskName = "增量同步北向持股";
                        var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:BXCGMX:*");
                        if (resKeys.SUCCESS)
                        {
                            var keys = resKeys.DATA as List<string>;
                            if (null != keys && 0 < keys.Count)
                            {
                                var startNewTask = true;
                                lock (this.threads增量同步)
                                {
                                    if (!this.threads增量同步.ContainsKey(taskName))
                                    {
                                        this.threads增量同步.Add(taskName, "Running");
                                        startNewTask = true;
                                    }
                                    else
                                    {
                                        startNewTask = false;
                                    }
                                }
                                if (startNewTask)
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            keys.ForEach(p =>
                                            {
                                                this.IncSyncBXCG(p);
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                            Thread.Sleep(10 * 1000);
                                        }
                                        finally
                                        {
                                            lock (this.threads增量同步)
                                            {
                                                if (this.threads增量同步.ContainsKey(taskName))
                                                {
                                                    this.threads增量同步.Remove(taskName);
                                                }
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"{taskName} 暂时没有要同步的数据");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"增量同步财务分析 检查任务异常 {e.Message}");
                    }
                    #endregion

                    #region 增量同步融资融券

                    try
                    {
                        var taskName = "增量同步融资融券";
                        var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:RZRQ:*");
                        if (resKeys.SUCCESS)
                        {
                            var keys = resKeys.DATA as List<string>;
                            if (null != keys && 0 < keys.Count)
                            {
                                var startNewTask = true;
                                lock (this.threads增量同步)
                                {
                                    if (!this.threads增量同步.ContainsKey(taskName))
                                    {
                                        this.threads增量同步.Add(taskName, "Running");
                                        startNewTask = true;
                                    }
                                    else
                                    {
                                        startNewTask = false;
                                    }
                                }
                                if (startNewTask)
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            keys.ForEach(p =>
                                            {
                                                this.IncSyncRZRQ(p);
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                            Thread.Sleep(10 * 1000);
                                        }
                                        finally
                                        {
                                            lock (this.threads增量同步)
                                            {
                                                if (this.threads增量同步.ContainsKey(taskName))
                                                {
                                                    this.threads增量同步.Remove(taskName);
                                                }
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"{taskName} 暂时没有要同步的数据");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"增量同步 检查任务异常 {e.Message}");
                    }
                    #endregion

                    #region 增量同步资金流向
                    try
                    {
                        var taskName = "增量同步资金流向";
                        var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:ZJLX:*");
                        if (resKeys.SUCCESS)
                        {
                            var keys = resKeys.DATA as List<string>;
                            if (null != keys && 0 < keys.Count)
                            {
                                var startNewTask = true;
                                lock (this.threads增量同步)
                                {
                                    if (!this.threads增量同步.ContainsKey(taskName))
                                    {
                                        this.threads增量同步.Add(taskName, "Running");
                                        startNewTask = true;
                                    }
                                    else
                                    {
                                        startNewTask = false;
                                    }
                                }
                                if (startNewTask)
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            keys.ForEach(p =>
                                            {
                                                this.IncSyncZJLX(p);
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                            Thread.Sleep(10 * 1000);
                                        }
                                        finally
                                        {
                                            lock (this.threads增量同步)
                                            {
                                                if (this.threads增量同步.ContainsKey(taskName))
                                                {
                                                    this.threads增量同步.Remove(taskName);
                                                }
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"{taskName} 暂时没有要同步的数据");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"增量同步 检查任务异常 {e.Message}");
                    }
                    #endregion

                    #region 北向代码同步
                    try
                    {
                        var setName = $"Stock:BXCode";
                        var qSyncName = $"Stock:Sync:2DB:{setName}";
                        var keyName = $"Stock:Sync:2DB:Check:{setName}";
                        var res = REDIS.Current.CacheGet<string>(keyName);
                        if (!res.SUCCESS)
                        {
                            this.同步北向代码();
                            REDIS.Current.CacheSet(keyName, DateTime.Now.ToString(), new TimeSpan(3, 0, 0));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"执行北向代码同步任务出现异常 {ex.Message}");
                    }
                    #endregion

                    #region 所有机构同步
                    try
                    {
                        var setName = $"Stock:JG";
                        var qSyncName = $"Stock:Sync:2DB:{setName}";
                        var keyName = $"Stock:Sync:2DB:Check:{setName}";
                        var res = REDIS.Current.CacheGet<string>(keyName);
                        if (!res.SUCCESS)
                        {
                            this.同步所有机构();
                            REDIS.Current.CacheSet(keyName, DateTime.Now.ToString(), new TimeSpan(24, 0, 0));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"执行所有机构同步任务出现异常 {ex.Message}");
                    }
                    #endregion

                    #region [废弃]增量同步北向持股统计
                    try
                    {
                        var taskName = "增量同步北向持股统计";
                        var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:BXCGTJ");
                        if (resKeys.SUCCESS)
                        {
                            var keys = resKeys.DATA as List<string>;
                            if (null != keys && 0 < keys.Count)
                            {
                                var startNewTask = true;
                                lock (this.threads增量同步)
                                {
                                    if (!this.threads增量同步.ContainsKey(taskName))
                                    {
                                        this.threads增量同步.Add(taskName, "Running");
                                        startNewTask = true;
                                    }
                                    else
                                    {
                                        startNewTask = false;
                                    }
                                }
                                if (startNewTask)
                                {
                                    Task.Run(() =>
                                    {
                                        try
                                        {
                                            keys.ForEach(p =>
                                            {
                                                this.IncSyncBXCGTJ(p);
                                            });
                                        }
                                        catch (Exception ex)
                                        {
                                            Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                            Thread.Sleep(10 * 1000);
                                        }
                                        finally
                                        {
                                            lock (this.threads增量同步)
                                            {
                                                if (this.threads增量同步.ContainsKey(taskName))
                                                {
                                                    this.threads增量同步.Remove(taskName);
                                                }
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    Console.WriteLine($"{taskName} 暂时没有要同步的数据");
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"增量同步 检查任务异常 {e.Message}");
                    }
                    #endregion
                     
                }

                #region 增量同步东方网快讯
                try
                {
                    var taskName = "增量同步东方网快讯";
                    var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:ShortNews");
                    if (resKeys.SUCCESS)
                    {
                        var keys = resKeys.DATA as List<string>;
                        if (null != keys && 0 < keys.Count)
                        {
                            var startNewTask = true;
                            lock (this.threads增量同步)
                            {
                                if (!this.threads增量同步.ContainsKey(taskName))
                                {
                                    this.threads增量同步.Add(taskName, "Running");
                                    startNewTask = true;
                                }
                                else
                                {
                                    startNewTask = false;
                                }
                            }
                            if (startNewTask)
                            {
                                Console.WriteLine($"{taskName} 同步线程准备启动");
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        keys.ForEach(p =>
                                        {
                                            this.IncSync<ShortNews>(p,(w)=> { return new object[] { w.Id }; });
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                        Thread.Sleep(10 * 1000);
                                    }
                                    finally
                                    {
                                        lock (this.threads增量同步)
                                        {
                                            if (this.threads增量同步.ContainsKey(taskName))
                                            {
                                                this.threads增量同步.Remove(taskName);
                                            }
                                        }
                                    }
                                });
                            }
                            else
                            {
                                Console.WriteLine($"{taskName} 同步线程正在运行或无需同步");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"增量同步 检查任务异常 {e.Message}");
                }
                #endregion

                #region 增量同步北向持股统计
                try
                {
                    var taskName = "增量同步北向持股统计";
                    var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:BXCGTJ");
                    if (resKeys.SUCCESS)
                    {
                        var keys = resKeys.DATA as List<string>;
                        if (null != keys && 0 < keys.Count)
                        {
                            var startNewTask = true;
                            lock (this.threads增量同步)
                            {
                                if (!this.threads增量同步.ContainsKey(taskName))
                                {
                                    this.threads增量同步.Add(taskName, "Running");
                                    startNewTask = true;
                                }
                                else
                                {
                                    startNewTask = false;
                                }
                            }
                            if (startNewTask)
                            {
                                Console.WriteLine($"{taskName} 同步线程准备启动");
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        keys.ForEach(p =>
                                        {
                                            this.IncSync<北向持股统计>(p, (w) => { return new object[] { w.Id }; });
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                        Thread.Sleep(10 * 1000);
                                    }
                                    finally
                                    {
                                        lock (this.threads增量同步)
                                        {
                                            if (this.threads增量同步.ContainsKey(taskName))
                                            {
                                                this.threads增量同步.Remove(taskName);
                                            }
                                        }
                                    }
                                });
                            }
                            else
                            {
                                Console.WriteLine($"{taskName} 同步线程正在运行或无需同步");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"增量同步 检查任务异常 {e.Message}");
                }
                #endregion

                #region 增量同步北向机构持股明细
                try
                {
                    var taskName = "增量同步北向机构持股明细";
                    var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:BXCGMXURL");
                    if (resKeys.SUCCESS)
                    {
                        var keys = resKeys.DATA as List<string>;
                        if (null != keys && 0 < keys.Count)
                        {
                            var startNewTask = true;
                            lock (this.threads增量同步)
                            {
                                if (!this.threads增量同步.ContainsKey(taskName))
                                {
                                    this.threads增量同步.Add(taskName, "Running");
                                    startNewTask = true;
                                }
                                else
                                {
                                    startNewTask = false;
                                }
                            }
                            if (startNewTask)
                            {
                                Console.WriteLine($"{taskName} 同步线程准备启动");
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        keys.ForEach(p =>
                                        {
                                            this.IncSync<北向机构持股明细>(p, (w) => { return new object[] { w.Id }; });
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                        Thread.Sleep(10 * 1000);
                                    }
                                    finally
                                    {
                                        lock (this.threads增量同步)
                                        {
                                            if (this.threads增量同步.ContainsKey(taskName))
                                            {
                                                this.threads增量同步.Remove(taskName);
                                            }
                                        }
                                    }
                                });
                            }
                            else
                            {
                                Console.WriteLine($"{taskName} 同步线程正在运行或无需同步");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"增量同步 检查任务异常 {e.Message}");
                }
                #endregion

                #region 增量同步融资融券
                try
                {
                    var taskName = "增量同步融资融券";
                    var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:RZRQ:*");
                    if (resKeys.SUCCESS)
                    {
                        var keys = resKeys.DATA as List<string>;
                        if (null != keys && 0 < keys.Count)
                        {
                            var startNewTask = true;
                            lock (this.threads增量同步)
                            {
                                if (!this.threads增量同步.ContainsKey(taskName))
                                {
                                    this.threads增量同步.Add(taskName, "Running");
                                    startNewTask = true;
                                }
                                else
                                {
                                    startNewTask = false;
                                }
                            }
                            if (startNewTask)
                            {
                                Console.WriteLine($"{taskName} 同步线程准备启动");
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        keys.ForEach(p =>
                                        {
                                            this.IncSync<融资融券>(p, (w) => { return new object[] { w.Id }; });
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                        Thread.Sleep(10 * 1000);
                                    }
                                    finally
                                    {
                                        lock (this.threads增量同步)
                                        {
                                            if (this.threads增量同步.ContainsKey(taskName))
                                            {
                                                this.threads增量同步.Remove(taskName);
                                            }
                                        }
                                    }
                                });
                            }
                            else
                            {
                                Console.WriteLine($"{taskName} 同步线程正在运行或无需同步");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"增量同步 检查任务异常 {e.Message}");
                }
                #endregion

                #region 增量同步资金流向
                try
                {
                    var taskName = "增量同步资金流向";
                    var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:ZJLX:*");
                    if (resKeys.SUCCESS)
                    {
                        var keys = resKeys.DATA as List<string>;
                        if (null != keys && 0 < keys.Count)
                        {
                            var startNewTask = true;
                            lock (this.threads增量同步)
                            {
                                if (!this.threads增量同步.ContainsKey(taskName))
                                {
                                    this.threads增量同步.Add(taskName, "Running");
                                    startNewTask = true;
                                }
                                else
                                {
                                    startNewTask = false;
                                }
                            }
                            if (startNewTask)
                            {
                                Console.WriteLine($"{taskName} 同步线程准备启动");
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        keys.ForEach(p =>
                                        {
                                            this.IncSync<资金流向>(p, (w) => { return new object[] { w.Id }; });
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                        Thread.Sleep(10 * 1000);
                                    }
                                    finally
                                    {
                                        lock (this.threads增量同步)
                                        {
                                            if (this.threads增量同步.ContainsKey(taskName))
                                            {
                                                this.threads增量同步.Remove(taskName);
                                            }
                                        }
                                    }
                                });
                            }
                            else
                            {
                                Console.WriteLine($"{taskName} 同步线程正在运行或无需同步");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"增量同步 检查任务异常 {e.Message}");
                }
                #endregion
                 
                #region 增量同步财务分析
                try
                {
                    var taskName = "增量同步财务分析";
                    var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:CWFX:*");
                    if (resKeys.SUCCESS)
                    {
                        var keys = resKeys.DATA as List<string>;
                        if (null != keys && 0 < keys.Count)
                        {
                            var startNewTask = true;
                            lock (this.threads增量同步)
                            {
                                if (!this.threads增量同步.ContainsKey(taskName))
                                {
                                    this.threads增量同步.Add(taskName, "Running");
                                    startNewTask = true;
                                }
                                else
                                {
                                    startNewTask = false;
                                }
                            }
                            if (startNewTask)
                            {
                                Console.WriteLine($"{taskName} 同步线程准备启动");
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        keys.ForEach(p =>
                                        {
                                            this.IncSync<财务主要指标>(p, (w) => { return new object[] { w.Id }; });
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                        Thread.Sleep(10 * 1000);
                                    }
                                    finally
                                    {
                                        lock (this.threads增量同步)
                                        {
                                            if (this.threads增量同步.ContainsKey(taskName))
                                            {
                                                this.threads增量同步.Remove(taskName);
                                            }
                                        }
                                    }
                                });
                            }
                            else
                            {
                                Console.WriteLine($"{taskName} 同步线程正在运行或无需同步");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"增量同步 检查任务异常 {e.Message}");
                }
                #endregion

                #region 增量同步北向成交明细
                try
                {
                    var taskName = "增量同步北向成交明细";
                    var resKeys = REDIS.Current.QueryKeys("Stock:Sync:2DB:Stock:BXCJMX");
                    if (resKeys.SUCCESS)
                    {
                        var keys = resKeys.DATA as List<string>;
                        if (null != keys && 0 < keys.Count)
                        {
                            var startNewTask = true;
                            lock (this.threads增量同步)
                            {
                                if (!this.threads增量同步.ContainsKey(taskName))
                                {
                                    this.threads增量同步.Add(taskName, "Running");
                                    startNewTask = true;
                                }
                                else
                                {
                                    startNewTask = false;
                                }
                            }
                            if (startNewTask)
                            {
                                Console.WriteLine($"{taskName} 同步线程准备启动");
                                Task.Run(() =>
                                {
                                    try
                                    {
                                        keys.ForEach(p =>
                                        {
                                            this.IncSync<北向成交明细>(p, (w) => { return new object[] { w.Code,w.日期tag }; });
                                        });
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                        Thread.Sleep(10 * 1000);
                                    }
                                    finally
                                    {
                                        lock (this.threads增量同步)
                                        {
                                            if (this.threads增量同步.ContainsKey(taskName))
                                            {
                                                this.threads增量同步.Remove(taskName);
                                            }
                                        }
                                    }
                                });
                            }
                            else
                            {
                                Console.WriteLine($"{taskName} 同步线程正在运行或无需同步");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"增量同步 检查任务异常 {e.Message}");
                }
                #endregion

                Console.WriteLine($"同步任务检查 {DateTime.Now}");
                Thread.Sleep(5000);
            } 
        }

        public void Run()
        { 
            this.AutoSync();
            

        }
    }
}
