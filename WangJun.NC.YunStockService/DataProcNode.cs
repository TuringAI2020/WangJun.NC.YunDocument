﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
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

        public RES SaveBXCGMX(string taskId, string jsonReq, string jsonRes)
        {
            try
            {
                var list = JSON.ToObject<List<北向持股明细>>(jsonRes);
                var count = 0;
                if (null != list && 0 < list.Count)
                {
                    var code = list.First().Code;
                    list.ForEach(p =>
                    {
                        var qSyncName = $"Stock:Sync:2DB:Stock:BXCGMX:{p.Code}";
                        var setName = $"Stock:BXCGMX:{p.Code}";
                        var res1 = REDIS.Current.Enqueue(qSyncName, p);
                        var res2 = REDIS.Current.SortedSetAdd(setName, p, p.持股日期tag);
                        var res = (res1.SUCCESS && res2.SUCCESS) ? RES.OK() : RES.FAIL();
                        if (res.SUCCESS)
                        {
                            count += 1;
                        }
                    });

                    taskDict[taskId].Remove(jsonReq);
                    if (count == list.Count)
                    {
                        var listName = $"Stock:Task:BXCGMX:{taskId}";
                        var res = REDIS.Current.RemoveListItem(listName, jsonReq, 1);
                        return res;
                    }
                    return RES.FAIL($"未完全更新 {code}");
                }
                return RES.FAIL($"传入参数无效或数据为空");
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex.Message);
            }
        }

        public RES SaveBXCJMX(string keyName, string taskId, string jsonReq, string jsonRes)
        {
            var listName = $"Stock:Task:{keyName}:{taskId}";

            try
            {
                var list = JSON.ToObject<List<北向成交明细>>(jsonRes);
                var count = 0;
                if (null != list && 0 < list.Count)
                {
                    var code = list.First().Code;
                    list.ForEach(p =>
                    {
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
                    taskDict[listName].Remove(jsonReq);
                    var res = REDIS.Current.RemoveListItem(listName, jsonReq, 1);
                    return res;
                }
                return RES.FAIL($"传入参数无效");
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex.Message);
            }
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
                        var code = list.First().Code;
                        list.ForEach(p =>
                        {
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
                        taskDict[listName].Remove(jsonReq);
                        var res = REDIS.Current.RemoveListItem(listName, jsonReq, 1);
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
                        taskDict[listName].Remove(jsonReq);
                        var res = REDIS.Current.RemoveListItem(listName, jsonReq, 1);
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
                        taskDict[listName].Remove(jsonReq);
                        var res = REDIS.Current.RemoveListItem(listName, jsonReq, 1);
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
                        taskDict[listName].Remove(jsonReq);
                        var res = REDIS.Current.RemoveListItem(listName, jsonReq, 1);
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

        private static Dictionary<string, Dictionary<string, int>> taskDict = new Dictionary<string, Dictionary<string, int>>();
        public RES GetTask(string taskId)
        {
            if (!taskDict.ContainsKey(taskId))
            {
                taskDict.Add(taskId, new Dictionary<string, int>());
            }

            if (taskDict[taskId].Count <= 15)
            {
                var res = REDIS.Current.GetListLastItems($"Stock:Task:BXCGMX:{taskId}", -1000);
                var list = res.DATA as List<string>;
                if (null != list)
                {
                    list.ForEach(p =>
                    {
                        if (!taskDict[taskId].ContainsKey(p))
                        {
                            taskDict[taskId].Add(p, 0);
                        }
                    });
                }
            }

            if (0 == taskDict[taskId].Values.Count(p => p == 0))
            {
                return RES.FAIL($"暂无可用任务,全部已分配{taskId} {taskDict[taskId].Count} { taskDict[taskId].Values.Count(p => p == 0)}");
            }

            var task = taskDict[taskId].First(p => p.Value == 0);
            taskDict[taskId][task.Key] = 1;
            Console.WriteLine($"任务分配情况 {taskId} {taskDict[taskId].Count} { taskDict[taskId].Values.Count(p => p == 0)}");
            return RES.OK(task.Key);
        }

        public RES GetTask(string keyName, string taskId)
        {
            var listName = $"Stock:Task:{keyName}:{taskId}";
            if (!taskDict.ContainsKey(listName))
            {
                taskDict.Add(listName, new Dictionary<string, int>());
            }

            if (taskDict[listName].Count <= 15)
            {
                var res = REDIS.Current.GetListLastItems(listName, -1000);
                var list = res.DATA as List<string>;
                if (null != list)
                {
                    list.ForEach(p =>
                    {
                        if (!taskDict[listName].ContainsKey(p))
                        {
                            taskDict[listName].Add(p, 0);
                        }
                    });
                }
            }

            if (0 == taskDict[listName].Values.Count(p => p == 0))
            {
                return RES.FAIL($"暂无可用任务,全部已分配{listName} {taskDict[listName].Count} { taskDict[listName].Values.Count(p => p == 0)}");
            }

            var task = taskDict[listName].First(p => p.Value == 0);
            taskDict[listName][task.Key] = 1;
            Console.WriteLine($"任务分配情况 {listName} {taskDict[listName].Count} { taskDict[listName].Values.Count(p => p == 0)}");
            return RES.OK(task.Key);
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
                var res = REDIS.Current.SortedSetTraverse(setName, "*", (dictName2, href, score, count2, index2) => {
                    var item =new {
                                             Score = score,
                                             Url= href,
                                             RetryCount=  3
                                            };
                    var res2 = REDIS.Current.Enqueue(qTaskName, item);
                    total++;
                    if (res2.SUCCESS) {
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
        }
}
