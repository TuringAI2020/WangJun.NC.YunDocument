using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WangJun.NC.YunStockService.Models;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunStockService
{
    public class DataProcNode
    {
        public static DataProcNode GetInst()
        {
            var inst = new DataProcNode();
            return inst;
        }


        public RES SaveProcData(string keyName, string taskId, string jsonReq, string jsonRes)
        {
            var listName = $"Stock:Task:{keyName}:{taskId}";

            try
            {
                if ("BXCJMX" == keyName)
                {
                    var count = 0;
                    var list = JSON.ToObject<List<北向成交明细>>(jsonRes);
                    if (null != list && 0 < list.Count)
                    {
                        list.ForEach(p =>
                        {
                            var qSyncName = $"Stock:Sync:2DB:Stock:{keyName}";
                            var setName = $"Stock:{keyName}:{p.Code}";
                            var res1 = REDIS.Current.Enqueue(qSyncName, p);
                            var res2 = REDIS.Current.SortedSetAdd(setName, p, p.日期tag);
                            var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                            if (res.SUCCESS)
                            {
                                count += 1;
                            }
                        });
                    }
                    if ((0 < list.Count && count == list.Count) || 0 == list.Count)
                    {
                        jsonReq = JSON.ToJson(JSON.ToObject<Dictionary<string, object>>(jsonReq));
                        var res = REDIS.Current.RemoveListItem(listName, jsonReq, -1);
                        this.RemoveTaskStatus(listName, jsonReq);
                        return res;
                    }
                }
                else if ("RZRQ" == keyName)
                {
                    var count = 0;
                    var list = JSON.ToObject<List<融资融券>>(jsonRes);
                    if (null != list && 0 < list.Count)
                    {
                        var code = list.First().Code;
                        list.ForEach(p =>
                        {
                            p.Id = GUID.FromStringToGuid($"{p.Code}_{p.交易日期tag}");
                            var qSyncName = $"Stock:Sync:2DB:Stock:{keyName}:{p.Code}";
                            var setName = $"Stock:{keyName}:{p.Code}";
                            var res1 = REDIS.Current.Enqueue(qSyncName, p);
                            var res2 = REDIS.Current.SortedSetAdd(setName, p, p.交易日期tag);
                            var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                            if (res.SUCCESS)
                            {
                                count += 1;
                            }
                        });
                    }
                    if ((0 < list.Count && count == list.Count) || 0 == list.Count)
                    {
                        jsonReq = JSON.ToJson(JSON.ToObject<Dictionary<string, object>>(jsonReq));
                        var res = REDIS.Current.RemoveListItem(listName, jsonReq, -1);
                        this.RemoveTaskStatus(listName, jsonReq);
                        return res;
                    }
                }
                else if ("ZJLX" == keyName)
                {
                    var count = 0;
                    var list = JSON.ToObject<List<资金流向>>(jsonRes);
                    if (null != list && 0 < list.Count)
                    {
                        var code = list.First().Code;
                        list.ForEach(p =>
                        {
                            p.Id = GUID.FromStringToGuid($"{p.Code}_{p.日期tag}");
                            var qSyncName = $"Stock:Sync:2DB:Stock:{keyName}:{p.Code}";
                            var setName = $"Stock:{keyName}:{p.Code}";
                            var res1 = REDIS.Current.Enqueue(qSyncName, p);
                            var res2 = REDIS.Current.SortedSetAdd(setName, p, p.日期tag);
                            var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                            if (res.SUCCESS)
                            {
                                count += 1;
                            }
                        });
                    }
                    if ((0 < list.Count && count == list.Count) || 0 == list.Count)
                    {
                        jsonReq = JSON.ToJson(JSON.ToObject<Dictionary<string, object>>(jsonReq));
                        var res = REDIS.Current.RemoveListItem(listName, jsonReq, -1);
                        this.RemoveTaskStatus(listName, jsonReq);
                        return res;
                    }
                }
                else if ("CWFX" == keyName)
                {
                    var count = 0;
                    jsonRes = jsonRes.Replace("(天)", string.Empty).Replace("(元)", string.Empty).Replace("(%)", string.Empty).Replace("/", string.Empty).Replace("(次)", string.Empty);
                    var list = JSON.ToObject<List<财务主要指标>>(jsonRes);
                    if (null != list && 0 < list.Count)
                    {
                        var code = list.First().Code;
                        list.ForEach(p =>
                        {
                            p.Id = GUID.FromStringToGuid($"{p.Code}_{p.DateTag}");
                            var qSyncName = $"Stock:Sync:2DB:Stock:{keyName}:{p.Code}";
                            var setName = $"Stock:{keyName}:{p.Code}";
                            var res1 = REDIS.Current.Enqueue(qSyncName, p);
                            var res2 = REDIS.Current.SortedSetAdd(setName, p, p.DateTag);
                            var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                            if (res.SUCCESS)
                            {
                                count += 1;
                            }
                        });
                    }
                    if ((0 < list.Count && count == list.Count) || 0 == list.Count)
                    {
                        jsonReq = JSON.ToJson(JSON.ToObject<Dictionary<string, object>>(jsonReq));
                        var res = REDIS.Current.RemoveListItem(listName, jsonReq, -1);
                        this.RemoveTaskStatus(listName, jsonReq);
                        return res;
                    }
                }
                else if ("BXCGTJ" == keyName)
                {
                    var count = 0;
                    var list = JSON.ToObject<List<北向持股统计>>(jsonRes);
                    if (null != list && 0 < list.Count)
                    {
                        list.ForEach(p =>
                        {
                            p.Id = GUID.FromStringToGuid($"{p.JgCode}_{p.持股日期tag}");
                            var qSyncName = $"Stock:Sync:2DB:Stock:{keyName}";
                            var setName = $"Stock:{keyName}:{p.机构名称}";
                            var res1 = REDIS.Current.Enqueue(qSyncName, p);
                            var res2 = REDIS.Current.SortedSetAdd(setName, p, p.持股日期tag);
                            var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                            if (res.SUCCESS)
                            {
                                count += 1;
                            }
                        });
                    }
                    if ((0 < list.Count && count == list.Count) || 0 == list.Count)
                    {
                        jsonReq = JSON.ToJson(JSON.ToObject<Dictionary<string, object>>(jsonReq));
                        var res = REDIS.Current.RemoveListItem(listName, jsonReq, -1);
                        this.RemoveTaskStatus(listName, jsonReq);
                        return res;
                    }
                }
                else if ("BXCGMXURL" == keyName)
                {
                    var count = 0;
                    var list = JSON.ToObject<List<北向机构持股明细>>(jsonRes);
                    if (null != list && 0 < list.Count)
                    {
                        list.ForEach(p =>
                        {
                            p.Id = GUID.FromStringToGuid($"{p.Code}_{p.持股日期tag}_{p.JgCode}");
                            var qSyncName = $"Stock:Sync:2DB:Stock:{keyName}";
                            var setName = $"Stock:{keyName}:{p.机构名称}";
                            var res1 = REDIS.Current.Enqueue(qSyncName, p);
                            var res2 = REDIS.Current.SortedSetAdd(setName, p, p.持股日期tag);
                            var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                            if (res.SUCCESS)
                            {
                                count += 1;
                            }
                        });
                    }
                    if ((0 < list.Count && count == list.Count) || 0 == list.Count)
                    {
                        jsonReq = JSON.ToJson(JSON.ToObject<Dictionary<string, object>>(jsonReq));
                        var res = REDIS.Current.RemoveListItem(listName, jsonReq, -1);
                        this.RemoveTaskStatus(listName, jsonReq);
                        return res;
                    }
                }
                return RES.FAIL($"传入参数无效");
            }
            catch (Exception ex)
            {
                return RES.FAIL($"SaveProcData {ex.Message}");
            }
        }


        public RES GetTask(string keyName, string taskId)
        {
            var listName = $"Stock:Task:{keyName}:{taskId}";

            var res = REDIS.Current.GetListLastItems(listName, -1000);
            var list = res.DATA as List<string>;
            if (null != list)
            {
                var res4 = RES.FAIL("尚未进行任务分配");
                var hasFound = false;
                list.ForEach(p =>
                {
                    if (!hasFound)
                    {
                        var res2 = this.IsExistTaskStatus(listName, JSON.ToJson(JSON.ToObject<Dictionary<string, object>>(p)));
                        if (!res2.SUCCESS)
                        {
                            var res3 = this.SaveTaskStatus(listName, p);
                            hasFound = true;
                            res4 = RES.OK(p);
                        }
                    }

                });
                return res4;
            }
            return RES.FAIL($"{listName} 为空");

        }

        protected RES SaveTaskStatus(string keyName, string val)
        {
            var id = GUID.FromStringToGuid(val);
            var taskStatusKeyName = $"Stock:Task:Status:{keyName}:{id}";
            var res = REDIS.Current.CacheSet(taskStatusKeyName, val, new TimeSpan(0, 5, 0));
            return res;
        }

        protected RES RemoveTaskStatus(string keyName, string val)
        {
            var id = GUID.FromStringToGuid(val);
            var taskStatusKeyName = $"Stock:Task:Status:{keyName}:{id}";
            var res = REDIS.Current.KeyRemove(taskStatusKeyName);
            return res;
        }

        protected RES IsExistTaskStatus(string keyName, string val)
        {
            var id = GUID.FromStringToGuid(val);
            var taskStatusKeyName = $"Stock:Task:Status:{keyName}:{id}";
            var res = REDIS.Current.QueryKeys(taskStatusKeyName);
            var keys = res.DATA as List<string>;
            if (keys.Contains(taskStatusKeyName))
            {
                return RES.OK(taskStatusKeyName);
            }
            return RES.FAIL(keys);
        }

        #region 北向代码更新
        public RES Update北向代码(string jsonReq, string jsonRes)
        {
            var setName = $"Stock:BXCode";
            var qSyncName = $"Stock:Sync:2DB:{setName}";

            try
            {
                var list = JSON.ToObject<List<北向代码>>(jsonRes);
                var count = 0;
                if (null != list && 0 < list.Count)
                {
                    list.ForEach(p =>
                    {
                        var res1 = REDIS.Current.Enqueue(qSyncName, p);
                        var res2 = REDIS.Current.DictAdd(setName, p.Code, p.Name);
                        var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                        if (res.SUCCESS)
                        {
                            count += 1;
                        }

                    });
                }
                Console.WriteLine($"{MethodBase.GetCurrentMethod().Name} 传入 {list.Count} 实际 {count}");
                return RES.OK(count, $"传入 {list.Count} 实际 {count}");
            }
            catch (Exception ex)
            {
                return RES.FAIL($"BXCODE {ex.Message}");
            }
        }
        #endregion

        #region 全部代码更新
        public RES UpdateALLCode(string jsonReq, string jsonRes)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;
            var setName = $"Stock:ALLCode";
            var qSyncName = $"Stock:Sync:2DB:{setName}";

            try
            {
                var list = JSON.ToObject<List<StockCode>>(jsonRes);
                var count = 0;
                if (null != list && 0 < list.Count)
                {
                    list.ForEach(p =>
                    {
                        var res1 = REDIS.Current.Enqueue(qSyncName, p);
                        var res2 = REDIS.Current.DictAdd(setName, p.Code, p.Name);
                        var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                        if (res.SUCCESS)
                        {
                            count += 1;
                        }

                    });
                }
                Console.WriteLine($"{methodName} 传入 {list.Count} 实际 {count}");
                return RES.OK(count, $"传入 {list.Count} 实际 {count}");
            }
            catch (Exception ex)
            {
                return RES.FAIL($"{methodName} {ex.Message}");
            }
        }
        #endregion

        #region 全部概念更新
        public RES UpdateALLConception(string jsonReq, string jsonRes)
        {
            var methodName = MethodBase.GetCurrentMethod().Name;
            var setName = $"Stock:ALLConception";
            var qSyncName = $"Stock:Sync:2DB:{setName}";

            try
            {
                var list = JSON.ToObject<List<Conception>>(jsonRes);
                var count = 0;
                if (null != list && 0 < list.Count)
                {
                    list.ForEach(p =>
                    {
                        var res1 = REDIS.Current.Enqueue(qSyncName, p);
                        var res2 = REDIS.Current.SetAdd(setName,p);
                        var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                        if (res.SUCCESS)
                        {
                            count += 1;
                        }

                    });
                }
                Console.WriteLine($"{methodName} 传入 {list.Count} 实际 {count}");
                return RES.OK(count, $"传入 {list.Count} 实际 {count}");
            }
            catch (Exception ex)
            {
                return RES.FAIL($"{methodName} {ex.Message}");
            }
        }
        #endregion

        #region 所有机构更新
        public RES Update所有机构(string jsonReq, string jsonRes)
        {
            var setName = $"Stock:JG";
            var qSyncName = $"Stock:Sync:2DB:{setName}";

            try
            {
                var list = JSON.ToObject<List<所有机构>>(jsonRes);
                var count = 0;
                if (null != list && 0 < list.Count)
                {
                    list.ForEach(p =>
                    {
                        var res1 = REDIS.Current.Enqueue(qSyncName, p);
                        var res2 = REDIS.Current.SortedSetAdd(setName, p.Href, p.Sort);
                        var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                        if (res.SUCCESS)
                        {
                            count += 1;
                        }

                    });
                }
                Console.WriteLine($"{MethodBase.GetCurrentMethod().Name} 传入 {list.Count} 实际 {count}");
                return RES.OK(count, $"传入 {list.Count} 实际 {count}");
            }
            catch (Exception ex)
            {
                return RES.FAIL($"JG {ex.Message}");
            }
        }
        #endregion

        #region 所有北向持股明细链接
        public RES Update所有北向持股明细链接(string jsonReq, string jsonRes)
        {
            try
            {
                var list = JSON.ToObject<List<Dictionary<string, string>>>(jsonRes);
                var count = 0;
                if (null != list && 0 < list.Count)
                {
                    list.ForEach(p =>
                    {
                        var dateTag = p["DateTag"];
                        var listName = $"Stock:Task:BXCGMXURL:{dateTag}";
                        var res = REDIS.Current.Enqueue(listName, p);
                        if (res.SUCCESS)
                        {
                            count += 1;
                        }
                    });
                }
                Console.WriteLine($"{MethodBase.GetCurrentMethod().Name} 传入 {list.Count} 实际 {count}");
                return RES.OK(count, $"传入 {list.Count} 实际 {count}");
            }
            catch (Exception ex)
            {
                return RES.FAIL($"Update所有北向持股明细链接 {ex.Message}");
            }
        }
        #endregion

        #region 东方财富网快讯更新
        public RES UpdateShortNews(string keyName, string jsonReq, string jsonRes)
        {
            var setName = $"Stock:ShortNews";
            var qSyncName = $"Stock:Sync:2DB:{setName}";

            try
            {
                var list = JSON.ToObject<List<ShortNews>>(jsonRes);
                var count = 0;
                if (null != list && 0 < list.Count)
                {
                    list.ForEach(p =>
                    {
                        p.Id = GUID.FromStringToGuid(p.Content);
                        var res1 = REDIS.Current.Enqueue(qSyncName, p);
                        var res2 = REDIS.Current.SortedSetAdd(setName, p, double.Parse(p.PublishTime.ToString("yyyyMMddhhmm")));
                        var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                        if (res.SUCCESS)
                        {
                            count += 1;
                        }

                    });
                }
                Console.WriteLine($"{MethodBase.GetCurrentMethod().Name} 传入 {list.Count} 实际 {count}");
                return RES.OK(count, $"传入 {list.Count} 实际 {count}");
            }
            catch (Exception ex)
            {
                return RES.FAIL($"BXCODE {ex.Message}");
            }
        }
        #endregion

        #region 长新闻更新
        public RES UpdateArticle(string keyName, string jsonReq, string jsonRes)
        {
            var setName = $"Stock:Article";
            var qSyncName = $"Stock:Sync:2DB:{setName}";
            var methodName = MethodBase.GetCurrentMethod().Name;
            try
            {
                var list = JSON.ToObject<List<Article>>(jsonRes);
                var count = 0;
                if (null != list && 0 < list.Count)
                {
                    list.ForEach(p =>
                    {
                        p.Id = GUID.FromStringToGuid(p.Content);
                        var res1 = REDIS.Current.Enqueue(qSyncName, p);
                        var res2 = REDIS.Current.SortedSetAdd(setName, p, double.Parse(p.PublishTime.ToString("yyyyMMddhhmm")));
                        var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                        if (res.SUCCESS)
                        {
                            count += 1;
                        }

                    });
                }
                Console.WriteLine($"{MethodBase.GetCurrentMethod().Name} 传入 {list.Count} 实际 {count}");
                return RES.OK(count, $"传入 {list.Count} 实际 {count}");
            }
            catch (Exception ex)
            {
                return RES.FAIL($"{methodName} {ex.Message}");
            }
        }
        #endregion


        #region 创建任务 - 北向持股统计
        public RES CreateTask北向持股统计(string jsonReq, string jsonRes)
        {
            var setName = $"Stock:JG";
            var qTaskName = "Stock:Task:BXCGTJ:Task0";

            try
            {
                var count = 0;
                var total = 0;
                REDIS.Current.KeyRemove(qTaskName);
                var res = REDIS.Current.SortedSetTraverse(setName, "*", (dictName2, href, score, count2, index2) =>
                {
                    var item = new
                    {
                        Score = score,
                        Url = href,
                        RetryCount = 3
                    };
                    var res2 = REDIS.Current.Enqueue(qTaskName, item);
                    total++;
                    if (res2.SUCCESS)
                    {
                        count++;
                    }
                    return true;
                });
                res.MESSAGE = $"北向持股统计任务创建完毕 应该创建 {total} 实际 {count}";
                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL($"CreateTask北向持股统计 {ex.Message}");
            }
        }
        #endregion

        #region 创建任务 - 融资融券
        public RES CreateTask融资融券(string jsonReq, string jsonRes)
        {
            var setName = $"Stock:BaseData:AllCode";
            var qTaskName = "Stock:Task:RZRQ:Task0";

            try
            {
                var count = 0;
                var total = 0;
                REDIS.Current.KeyRemove(qTaskName);
                var res = REDIS.Current.DictTraverse(setName, "*", (dictName, code, name, count2, index) =>
                {
                    var item = new
                    {
                        Code = code,
                        Url = $"http://data.eastmoney.com/rzrq/detail/{code}.html",
                        RetryCount = 3
                    };
                    var res2 = REDIS.Current.Enqueue(qTaskName, item);
                    total++;
                    if (res2.SUCCESS)
                    {
                        count++;
                    }
                    return true;
                });
                res.MESSAGE = $"{MethodBase.GetCurrentMethod().Name} 任务创建完毕 应该创建 {total} 实际 {count}";
                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL($"{MethodBase.GetCurrentMethod().Name} 任务创建异常 {ex.Message}");
            }
        }
        #endregion

        #region 创建任务 - 资金流向
        public RES CreateTask资金流向(string jsonReq, string jsonRes)
        {
            var setName = $"Stock:BaseData:AllCode";
            var qTaskName = "Stock:Task:ZJLX:Task0";

            try
            {
                var count = 0;
                var total = 0;
                REDIS.Current.KeyRemove(qTaskName);
                var res = REDIS.Current.DictTraverse(setName, "*", (dictName, code, name, count2, index) =>
                {
                    var item = new
                    {
                        Code = code,
                        Url = $"http://data.eastmoney.com/zjlx/{code}.html",
                        RetryCount = 3
                    };
                    var res2 = REDIS.Current.Enqueue(qTaskName, item);
                    total++;
                    if (res2.SUCCESS)
                    {
                        count++;
                    }
                    return true;
                });
                res.MESSAGE = $"{MethodBase.GetCurrentMethod().Name} 任务创建完毕 应该创建 {total} 实际 {count}";
                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL($"{MethodBase.GetCurrentMethod().Name} 任务创建异常 {ex.Message}");
            }
        }
        #endregion

        #region 创建任务 - 财务分析
        public RES CreateTask财务分析(string jsonReq, string jsonRes)
        {
            var setName = $"Stock:BaseData:AllCode";
            var qTaskName = "Stock:Task:CWFX:Task0";

            try
            {
                var count = 0;
                var total = 0;
                REDIS.Current.KeyRemove(qTaskName);
                var res = REDIS.Current.DictTraverse(setName, "*", (dictName, code, name, count2, index) =>
                {
                    var code2 = (code[0] == '6') ? "SH" + code : "SZ" + code;
                    var item = new
                    {
                        Code = code,
                        Url = $"http://f10.eastmoney.com/f10_v2/FinanceAnalysis.aspx?code={code2}",
                        RetryCount = 3
                    };
                    var res2 = REDIS.Current.Enqueue(qTaskName, item);
                    total++;
                    if (res2.SUCCESS)
                    {
                        count++;
                    }
                    return true;
                });
                res.MESSAGE = $"{MethodBase.GetCurrentMethod().Name} 任务创建完毕 应该创建 {total} 实际 {count}";
                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL($"{MethodBase.GetCurrentMethod().Name} 任务创建异常 {ex.Message}");
            }
        }
        #endregion

        #region 创建任务 - 北向成交明细
        public RES CreateTask北向成交明细(string jsonReq, string jsonRes)
        {
            var setName = $"Stock:BXCode";
            var qTaskName = "Stock:Task:BXCJMX:Task0";

            try
            {
                var count = 0;
                var total = 0;
                REDIS.Current.KeyRemove(qTaskName);
                var res = REDIS.Current.DictTraverse(setName, "*", (dictName, code, name, count2, index) =>
                {

                    var item = new
                    {
                        Code = code,
                        Url = $"http://data.eastmoney.com/hsgt/{code}.html",
                        RetryCount = 3
                    };
                    var res2 = REDIS.Current.Enqueue(qTaskName, item);
                    total++;
                    if (res2.SUCCESS)
                    {
                        count++;
                    }
                    return true;
                });
                res.MESSAGE = $"{MethodBase.GetCurrentMethod().Name} 任务创建完毕 应该创建 {total} 实际 {count}";
                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL($"{MethodBase.GetCurrentMethod().Name} 任务创建异常 {ex.Message}");
            }
        }
        #endregion
    }
}
