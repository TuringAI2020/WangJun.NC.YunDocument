using System;
using WangJun.NC.YunDocument.Models;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunDocument
{
    /// <summary>
    /// 同步工具
    /// </summary>
    public class SyncTaskRunner
    {

        public void SyncToBackDB<T1,T2>(Action<T2, T2> callback) where T1: Item, new() where T2:class, new()
        {
            //{
            //    var qName = $"MODIFY:{typeof(T1).FullName}".Replace(".", ":");
            //    var listName = $"LIST:{typeof(T1).FullName}".Replace(".", ":");
            //    var resCount = REDIS.Current.GetQueueLength(qName);
            //    var count = Convert.ToInt32(resCount.DATA);
            //    while (resCount.SUCCESS && 0 < count)
            //    {
            //        var popRes = REDIS.Current.Dequeue<Item>(qName);
            //        if (popRes.SUCCESS && null != popRes.DATA && popRes.DATA is Item)
            //        {
            //            var qItem = JSON.Convert<Item, T1>(popRes.DATA as Item);
            //            var val = RedisDB.Current.QueryItem<T1>(qItem);
            //            var newVal = JSON.ToObject<T2>(val.DATA.ToString());
            //            BackDB.Current.Save<T2>(newVal, callback, new object[] { qItem.ItemID });
            //        }
            //        resCount = REDIS.Current.GetQueueLength(qName);
            //        count = Convert.ToInt32(resCount.DATA);
            //    }
            //}

            {
                var qName = $"DELETE:{typeof(T1).FullName}".Replace(".", ":");
                var listName = $"LIST:{typeof(T1).FullName}".Replace(".", ":");
                var resCount = REDIS.Current.GetQueueLength(qName);
                var count = Convert.ToInt32(resCount.DATA);
                while (resCount.SUCCESS && 0 < count)
                {
                    var popRes = REDIS.Current.Dequeue<Item>(qName);
                    if (popRes.SUCCESS && null != popRes.DATA && popRes.DATA is Item)
                    {
                        var qItem = JSON.Convert<Item, T1>(popRes.DATA as Item);
                        var val = RedisDB.Current.QueryItem<T1>(qItem);
                        var newVal = JSON.ToObject<T2>(val.DATA.ToString());
                        BackDB.Current.Remove<T2>(new object[] { qItem.ItemID });
                    }
                    resCount = REDIS.Current.GetQueueLength(qName);
                    count = Convert.ToInt32(resCount.DATA);
                }
            }
        }
    }
}
