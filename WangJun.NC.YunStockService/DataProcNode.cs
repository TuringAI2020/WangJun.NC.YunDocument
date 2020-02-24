﻿using System;
using System.Collections.Generic;
using System.Linq;
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

        public RES SaveBXCGMX(string taskId,string jsonReq, string jsonRes)
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
                    
                    taskList.Remove(jsonReq);
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

        private static Dictionary<string,int> taskList = new Dictionary<string, int>();
        public RES GetTask(string taskId)
        {
            if (taskList.Count<=15)
            {
                var res = REDIS.Current.GetListLastItems($"Stock:Task:BXCGMX:{taskId}",-1000);
                var list = res.DATA as List<string>;
                if (null != list)
                {
                    list.ForEach(p=> {
                        if (!taskList.ContainsKey(p))
                        {
                            taskList.Add(p, 0);
                        }
                    });
                }
            }

            if (0 == taskList.Values.Count(p=>p==0))
            {
                return RES.FAIL($"暂无可用任务,全部已分配 {taskList.Count} { taskList.Values.Count(p => p == 0)}");
            }

            var task = taskList.First(p => p.Value == 0);
            taskList[task.Key] = 1;
            Console.WriteLine($"任务分配情况 {taskList.Count} { taskList.Values.Count(p => p == 0)}");
            return RES.OK(task.Key);
        }
    }
}
