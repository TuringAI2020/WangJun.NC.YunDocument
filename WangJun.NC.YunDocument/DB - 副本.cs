using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WangJun.NC.YunDocument.Models;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunDocument
{
    public class DB
    {
        private static WangJunDocumentContext db = new WangJunDocumentContext ();

        private static Dictionary<string, List<PropertyInfo>> proerptyDict = new Dictionary<string, List<PropertyInfo>>();
        public static DbSet<Document> Document
        {
            get
            {
                if (db == null)
                {
                    db = new WangJunDocumentContext();
                }
                return db.Document;
            }
        }

        public static Task<int> SaveEntity<T>(T t) where T : class
        {
            if (null != t)
            {
                var pTable = db.GetType().GetProperty(t.GetType().Name);
                var table = pTable.GetValue(db) as DbSet<T>;

                if (null != table)
                {
                    var classFullName = $"{t.GetType().FullName}";

                    if (!DB.proerptyDict.ContainsKey(classFullName))
                    {
                        var propertyList = t.GetType().GetProperties().ToList();
                        DB.proerptyDict.Add(classFullName, propertyList);
                    }
                    var properties = DB.proerptyDict[classFullName];

                    var isNew = false;
                    #region 检查
                    properties.Where(p => p.CanWrite && p.CanRead).ToList().ForEach(p =>
                    {
                        var keyAttr = p.GetCustomAttributes(typeof(KeyAttribute), false).FirstOrDefault() as KeyAttribute;
                        if (keyAttr is KeyAttribute)
                        {
                            var val = p.GetValue(t, null);
                            if (val != null &&( val.ToString() == Guid.Empty.ToString() || "0" == val.ToString()))
                            {
                                if (val.GetType() == typeof(Guid))
                                {
                                    val = Guid.NewGuid();
                                    p.SetValue(t, val);
                                    isNew = true;
                                }
                            }
                        }

                        var editableAttr = p.GetCustomAttributes(typeof(EditableAttribute), false).FirstOrDefault() as EditableAttribute;
                        if (editableAttr is EditableAttribute)
                        {
                            var val = p.GetValue(t, null);
                            if (val == null && p.PropertyType == typeof(string))
                            {
                                //p.SetValue(t, $"Test");
                            }
                        }
                    });

                    #endregion

                    if (isNew)
                    {
                        table.Add(t);
                    }
                    else
                    {
                        var trackEntry = db.Entry(t);
                        properties.ForEach(p =>
                        {
                            var editableAttr = p.GetCustomAttributes(typeof(EditableAttribute), false).FirstOrDefault() as EditableAttribute;
                            if (editableAttr is EditableAttribute)
                            {
                                var val = p.GetValue(t, null);
                                if (val == null && p.PropertyType == typeof(string))
                                {
                                   
                                    trackEntry.Property(p.Name).IsModified = true;
                                }
                            }
                        });
                        trackEntry.State = EntityState.Modified; 
                    }
                }
                var task = db.SaveChangesAsync();
                return task;
            }

            return null;
        }

        public static Task<int> DeleteEntity<T>(T t) where T : class
        {
            if (null != t)
            {
                var pTable = db.GetType().GetProperty(t.GetType().Name);
                var table = pTable.GetValue(db) as DbSet<T>;

                if (null != table)
                {
                    var classFullName = $"{t.GetType().FullName}";

                    if (!DB.proerptyDict.ContainsKey(classFullName))
                    {
                        var propertyList = t.GetType().GetProperties().ToList();
                        DB.proerptyDict.Add(classFullName, propertyList);
                    }

                    var properties = DB.proerptyDict[classFullName];
                    var trackEntry = db.Entry(t);
                    properties.ForEach(p =>
                    {
                        var editableAttr = p.GetCustomAttributes(typeof(EditableAttribute), false).FirstOrDefault() as EditableAttribute;
                        if (editableAttr is KeyAttribute)
                        {
                            var val = p.GetValue(t, null);
                            if (val == null && p.PropertyType == typeof(string))
                            {

                                trackEntry.Property(p.Name).IsModified = true;
                            }
                        }
                    });

                    table.Remove(t);

                    var task = DB.db.SaveChangesAsync();
                    return task;
                }
            }
            return null;
        }

        public static RES SyncDB<T1,T2>(string redis=null) where T1 :class
                                                                                            where T2 :class
        {
            var qName = $"MODIFY:{typeof(T1).FullName.Replace(".", ":")}";
            var dictName = $"LIST:{typeof(T1).FullName.Replace(".", ":")}";
            var res = REDIS.Current.GetQueueLength(qName);
            if (res.SUCCESS)
            {
                var length = (long)res.DATA;
                while (0 < length)
                {
                    var resItem = REDIS.Current.Dequeue<Item>(qName);
                    if (resItem.SUCCESS&& resItem.DATA is Item)
                    {
                        var sourceItem = resItem.DATA as Item;
                        var sourceJson = REDIS.Current.DictGet(dictName, sourceItem.ItemID.ToString());
                        var targetItem = JSON.ToObject<T2>(sourceJson.DATA as string);
                        var task = DB.SaveEntity<T2>(targetItem);
                        Console.WriteLine(task.Result);
                        if (length == 1)
                        {
                            Console.WriteLine($"同步完毕{task.Result}");
                        }
                    }
                    res = REDIS.Current.GetQueueLength(qName);
                    length = (long)res.DATA;
                }
            }
            return RES.OK("全部同步完毕");
        }
    }
}
