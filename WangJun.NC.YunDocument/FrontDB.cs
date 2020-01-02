using System;
using System.Collections.Generic;
using System.Text;
using WangJun.NC.YunDocument.Front;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunDocument
{
    public class FrontDB
    {
        private static FrontDB db=null;
        public static FrontDB Current
        {
            get
            {
                if (null == db)
                {
                    db = new FrontDB();
                }
                return db;
            }
        }

        /// <summary>
        /// 保存一个文档
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public RES Save<T>(string data) where T: Item,new()
        {
            try
            {
                var checkRes = this.CheckBeforeSave<T>(data);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var inst = checkRes.DATA as T;

               var res =  RedisDB.Current.Save<T>(inst);


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

        /// <summary>
        /// 移除一个文档
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public RES Remove<T>(string data) where T : Item, new()
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

        public RES QueryList<T>(string filter,Action<T> callback=null) where T : Item, new()
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
                if (null != list &&null != callback)
                {
                    list.ForEach(p=> {
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

        public RES QueryItem<T>(string data) where T : Item, new()
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
