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
        public RES Save(string data)
        {
            try
            {
                var checkRes = this.CheckBeforeSave(data);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var inst = checkRes.DATA as Document;

               var res =  RedisDB.Current.Save<Document>(inst);


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
        protected RES CheckBeforeSave(string data)
        {
            try
            {
                var inst = JSON.ToObject<Document>(data);
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
        public RES Remove(string data)
        {
            try
            {
                var checkRes = this.CheckBeforeRemove(data);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var inst = checkRes.DATA as Document;

                var res = RedisDB.Current.Remove<Document>(inst);


                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        protected RES CheckBeforeRemove(string data)
        {
            try
            {
                var inst = JSON.ToObject<Document>(data);
                return RES.OK(inst);
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        public RES QueryList(string filter)
        {
            try
            {
                var checkRes = this.CheckBeforeQueryList(filter);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var queryFilter = checkRes.DATA as QueryFilter;

                var res = RedisDB.Current.QueryList<Document>(queryFilter.ToDictionary());
                var list = res.DATA as List<Document>;
                list.ForEach(p =>
                {
                    p.Detail = null;
                });
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

        public RES QueryItem(string data)
        {
            try
            {
                var checkRes = this.CheckBeforeQueryItem(data);
                if (!checkRes.SUCCESS)
                {
                    return checkRes;
                }

                var inst = checkRes.DATA as Document;

                var res = RedisDB.Current.QueryItem<Document>(inst);


                return res;
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        protected RES CheckBeforeQueryItem(string data)
        {
            try
            {
                var inst = JSON.ToObject<Document>(data);
                return RES.OK(inst);
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }
    }
}
