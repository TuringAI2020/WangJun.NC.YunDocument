using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TencentCloud.Nlp.V20190408.Models;
using WangJun.NC.YunStockService.Models;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunStockService
{
    /// <summary>
    /// 同步工具
    /// </summary>
    public class SyncTaskRunner
    {

        public static SyncTaskRunner GetInst()
        {
            var inst = new SyncTaskRunner();
            return inst;
        }

        #region 旧代码  

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
                        var dbRes = db.Save<所有机构>(jsonData, null, new object[] { jsonData.JgCode, jsonData.JgName });
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
        #endregion





        #region 增量同步函数
        protected void IncSync<T>(string qName, Func<T, object[]> callbackGetKey) where T : class, new()
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

        #region 增量同步任务函数
        protected void ExecuteIncSyncTask<T>(string taskName, string redisSourceKeys, Func<T, object[]> keycallback) where T : class, new()
        {
            try
            {
                var resKeys = REDIS.Current.QueryKeys(redisSourceKeys);
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
                                        this.IncSync<T>(p, keycallback);
                                    });
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"{taskName} 任务处理异常: {ex.Message}");
                                    Thread.Sleep(20 * 1000);
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
                Thread.Sleep(20 * 1000);
                Console.WriteLine($"增量同步 检查任务异常 {e.Message}");
            }
        }
        #endregion 

        protected Dictionary<string, string> threads增量同步 = new Dictionary<string, string>();
        protected void AutoSync()
        { 
            while (true)
            {
                if (false)
                { 
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
                }
                 
                this.ExecuteIncSyncTask<ShortNews>("增量同步东方网快讯", "Stock:Sync:2DB:Stock:ShortNews", (w) => {
                    var resWords = AI_T.GetInst().KeywordsExtraction(w.Content);
                    if (resWords.SUCCESS && resWords.DATA is List<Keyword>)
                    {
                        var words = resWords.DATA as List<Keyword>;
                        var setName = "Stock:Keywords:ALL";
                        var qSyncName = $"Stock:Sync:2DB:{setName}";
                        words.ForEach(m =>
                        {
                            REDIS.Current.Enqueue(qSyncName, new Keywords { Id = GUID.FromStringToGuid(m.Word), Keyword = m.Word });
                            var res = REDIS.Current.SetAdd(setName, m.Word);
                        });
                    }
                    return new object[] { w.Id }; 
                });

                this.ExecuteIncSyncTask<北向持股统计>("增量同步北向持股统计", "Stock:Sync:2DB:Stock:BXCGTJ", (w) => {
                    return new object[] { w.Id };
                });

                this.ExecuteIncSyncTask<北向机构持股明细>("增量同步北向机构持股明细", "Stock:Sync:2DB:Stock:BXCGMXURL", (w) => {
                    return new object[] { w.Id };
                });

                this.ExecuteIncSyncTask<融资融券>($"增量同步{nameof(融资融券)}", "Stock:Sync:2DB:Stock:RZRQ:*", (w) => {
                    return new object[] { w.Id };
                });

                this.ExecuteIncSyncTask<资金流向>($"增量同步{nameof(资金流向)}", "Stock:Sync:2DB:Stock:ZJLX:*", (w) => {
                    return new object[] { w.Id };
                });

                this.ExecuteIncSyncTask<财务主要指标>("增量同步财务分析", "Stock:Sync:2DB:Stock:CWFX:*", (w) => {
                    return new object[] { w.Id };
                });

                this.ExecuteIncSyncTask<北向成交明细>("增量同步北向成交明细", "Stock:Sync:2DB:Stock:BXCJMX", (w) => {
                    return new object[] { w.Code, w.日期tag };
                });

                this.ExecuteIncSyncTask<Keywords>("增量同步关键词", "Stock:Sync:2DB:Stock:Keywords:ALL", (w) => { 
                    return new object[] { w.Id }; 
                });

                this.ExecuteIncSyncTask<北向代码>($"增量同步{nameof(北向代码)}", "Stock:Sync:2DB:Stock:BXCode", (w) => {
                    return new object[] { w.Code };
                });

                this.ExecuteIncSyncTask<StockCode>($"增量同步{nameof(StockCode)}", "Stock:Sync:2DB:Stock:ALLCode", (w) => {
                    return new object[] { w.Code };
                });
                this.ExecuteIncSyncTask<Conception>($"增量同步{nameof(Conception)}", "Stock:Sync:2DB:Stock:ALLConception", (w) => {
                    return new object[] { w.Name };
                });

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
