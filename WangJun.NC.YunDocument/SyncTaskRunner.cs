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
        #region 文档部分
        protected RES SaveDocument(string data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(data) || !(data.StartsWith("{") && data.StartsWith("}")))
                {
                    return RES.FAIL("传入参数非法");
                }

                var doc = JSON.ToObject<Document>(data);
                if (null == doc)
                {
                    return RES.FAIL("无有效业务参数");
                }

                if (doc.Id == Guid.Empty)
                {
                    doc.Id = ID.GUID;
                    doc.CreateTime = DateTime.Now;
                    doc.UpdateTime = DateTime.Now;
                    DB.Document.Add(doc);
                }
                else
                {
                    //DB.Document.
                }

                return RES.OK();
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        protected RES RemoveDocument(string filter)
        {
            try
            {

                return RES.OK();
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        protected RES DeleteDocument(string filter)
        {
            try
            {

                return RES.OK();
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        protected RES SyncCheck()
        {
            try
            {

                return RES.OK();
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }

        protected RES InitialFront()
        {
            try
            {

                return RES.OK();
            }
            catch (Exception ex)
            {
                return RES.FAIL(ex);
            }
        }
        #endregion
    }
}
