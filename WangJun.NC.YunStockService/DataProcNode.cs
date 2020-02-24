using System;
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
         
        private static Dictionary<string, Dictionary<string, int>> taskDict = new Dictionary<string, Dictionary<string, int>>();
        public RES GetTask(string taskId)
        {
            if (!taskDict.ContainsKey(taskId)){
                taskDict.Add(taskId, new Dictionary<string, int>());
            }
   
            if (taskDict[taskId].Count<=15)
            {
                var res = REDIS.Current.GetListLastItems($"Stock:Task:BXCGMX:{taskId}",-1000);
                var list = res.DATA as List<string>;
                if (null != list)
                {
                    list.ForEach(p=> {
                        if (!taskDict[taskId].ContainsKey(p))
                        {
                            taskDict[taskId].Add(p, 0);
                        }
                    });
                }
            }

            if (0 == taskDict[taskId].Values.Count(p=>p==0))
            {
                return RES.FAIL($"暂无可用任务,全部已分配{taskId} {taskDict[taskId].Count} { taskDict[taskId].Values.Count(p => p == 0)}");
            }

            var task = taskDict[taskId].First(p => p.Value == 0);
            taskDict[taskId][task.Key] = 1;
            Console.WriteLine($"任务分配情况 {taskId} {taskDict[taskId].Count} { taskDict[taskId].Values.Count(p => p == 0)}");
            return RES.OK(task.Key);
        }
    }
}
