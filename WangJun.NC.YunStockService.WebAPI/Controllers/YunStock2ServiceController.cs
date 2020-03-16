using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunStockService.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class YunStock2ServiceController : ControllerBase
    { 

        private readonly ILogger<YunStock2ServiceController> _logger;

        public YunStock2ServiceController(ILogger<YunStock2ServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public RES Get(string keyName, string taskId)
        {
            var res =  DataProcNode.GetInst().GetTask(keyName,taskId);
            return res;
        }

        [HttpPost]
        public RES Post()
        {
            var keyName = this.Request.Form["keyName"];
            var method = this.Request.Form["method"];
            var taskId = this.Request.Form["taskId"];
            var jsonReq = this.Request.Form["jsonReq"];
            var jsonRes = this.Request.Form["jsonRes"];
            var res = RES.FAIL("keyName参数未匹配");
            if ("RZRQ" == keyName || "ZJLX" == keyName || "CWFX" == keyName || "BXCGTJ" == keyName
                || "BXCGMXURL" == keyName|| "BXCJMX" == keyName)
            {
                res = DataProcNode.GetInst().SaveProcData(keyName, taskId, jsonReq, jsonRes);
            }
            else if ("BXCODE" == keyName)
            {
                res = DataProcNode.GetInst().Update北向代码(jsonReq, jsonRes);
            }
            else if ("JG" == keyName)
            {
                res = DataProcNode.GetInst().Update所有机构(jsonReq, jsonRes);
            }
            else if ("BXCGMXURLTASK" == keyName)
            {
                res = DataProcNode.GetInst().Update所有北向持股明细链接(jsonReq, jsonRes);
            }
            else if ("SHORTNEWS" == keyName)
            {
                res = DataProcNode.GetInst().UpdateShortNews(keyName,jsonReq, jsonRes);
            }
            return res;
        }
    }
}
