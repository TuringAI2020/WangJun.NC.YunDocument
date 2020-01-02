using System;
using System.Collections.Generic;
using System.Text;
using WangJun.NC.YunUtils;
using WangJun.NC.YunDocument.Front;

namespace WangJun.NC.YunDocument
{
    public class CategoryFrontDB
    {

        private static CategoryFrontDB db = null;
        public static CategoryFrontDB Current
        {
            get
            {
                if (null == db)
                {
                    db = new CategoryFrontDB();
                }
                return db;
            }
        }
        public RES CreateRootNode<T>(string data) where T : Category, new()
        {
            try
            {
                var checkRes = this.CheckBeforeSave<T>(data);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var inst = checkRes.DATA as T;

                var res = RedisDB.Current.Save<T>(inst);
                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        /// <summary>
        /// 保存前业务数据检查
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected RES CheckBeforeSave<T>(string data) where T : Item, new()
        {
            try
            {
                var inst = JSON.ToObject<T>(data);
                return RES.OK(inst);
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        public RES CreateSubNode<T>(string data) where T : Category, new()
        {
            try
            {
                var checkRes = this.CheckBeforeSave<T>(data);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var inst = checkRes.DATA as T;

                var res = RedisDB.Current.Save<T>(inst);
                var key = $"PARENT:{typeof(T).FullName.Replace(".", ":")}";
                REDIS.Current.DictAdd(key,inst.ItemID.ToString(), inst.ParentId.ToString());

                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        public RES RenameNodeName<T>(string data) where T : Category, new()
        {
            try
            {
                var checkRes = this.CheckBeforeSave<T>(data);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var inst = checkRes.DATA as T;

                var res = RedisDB.Current.Save<T>(inst);

                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        public RES RemoveNode<T>(string data) where T : Category, new()
        {
            try
            {
                var checkRes = this.CheckBeforeRemove<T>(data);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var inst = checkRes.DATA as T;

                var res = RedisDB.Current.Remove<T>(inst);
                var classFullName = typeof(T).FullName.Replace(".", ":") ;
                var key = $"PARENT:{typeof(T).FullName.Replace(".", ":")}";
                var resParent = REDIS.Current.DictGet(key, inst.ItemID.ToString());
                if (resParent.SUCCESS)
                {
                    var parentId =  resParent.DATA.ToString() ;
                    var indexKey = $"INDEX:{classFullName}:ParentId";
                    var resIndex = REDIS.Current.DictGet(indexKey, parentId);
                    if (resIndex.SUCCESS)
                    {
                        var childList = JSON.ToObject<List<string>>(resIndex.DATA.ToString());
                        childList.Remove(inst.ItemID.ToString());
                        REDIS.Current.DictAdd(indexKey, parentId, childList);
                    }
                }
                REDIS.Current.DictRemove(key, inst.ItemID.ToString());


                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        protected RES CheckBeforeRemove<T>(string data) where T : Item, new()
        {
            try
            {
                var inst = JSON.ToObject<T>(data);
                return RES.OK(inst);
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        public RES MoveNode<T>(string data) where T : Category, new()
        {
            try
            {
                var checkRes = this.CheckBeforeSave<T>(data);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var inst = checkRes.DATA as T;

                var res = RedisDB.Current.Save<T>(inst);
                var key = $"PARENT:{typeof(T).FullName.Replace(".", ":")}";
                REDIS.Current.DictAdd(key, inst.ItemID.ToString(), inst.ParentId.ToString());

                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        public RES GetSubNodes<T>(string filter, Action<T> callback = null) where T : Category, new()
        {
            try
            {
                var checkRes = this.CheckBeforeQueryList(filter);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var queryFilter = checkRes.DATA as QueryFilter;

                var res = RedisDB.Current.QueryList<T>(queryFilter.ToDictionary());
                var list = res.DATA as List<T>;
                if (null != list && null != callback)
                {
                    list.ForEach(p => {
                        callback(p);
                    });
                }
                return RES.OK(list);
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        protected RES CheckBeforeQueryList<T>(string data) where T : Category, new()
        {
            try
            {
                var inst = JSON.ToObject<QueryFilter>(data);
                return RES.OK(inst);
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        public RES GetTree<T>(string filter, Action<T> callback = null) where T : Category, new()
        {
            try
            {
                var checkRes = this.CheckBeforeQueryList(filter);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var queryFilter = checkRes.DATA as QueryFilter;

                var res = RedisDB.Current.QueryList<T>(queryFilter.ToDictionary());
                var list = res.DATA as List<T>;
                if (null != list && null != callback)
                {
                    list.ForEach(p => {
                        callback(p);
                    });
                }
                return RES.OK(list);
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        protected RES CheckBeforeQueryList(string data)
        {
            try
            {
                var inst = JSON.ToObject<QueryFilter>(data);
                return RES.OK(inst);
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        public RES GetNode<T>(string data) where T : Item, new()
        {
            try
            {
                var checkRes = this.CheckBeforeQueryItem<T>(data);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var inst = checkRes.DATA as T;

                var res = RedisDB.Current.QueryItem<T>(inst);


                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        protected RES CheckBeforeQueryItem<T>(string data) where T : Item, new()
        {
            try
            {
                var inst = JSON.ToObject<T>(data);
                return RES.OK(inst);
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }
    }
}
