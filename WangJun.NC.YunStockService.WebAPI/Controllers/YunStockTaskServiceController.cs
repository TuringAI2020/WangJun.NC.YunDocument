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
    public class YunStockTaskServiceController : ControllerBase
    { 

        private readonly ILogger<YunStockTaskServiceController> _logger;

        public YunStockTaskServiceController(ILogger<YunStockTaskServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public RES Get(string keyName, string taskId)
        {
            
            return RES.OK();
        }

        [HttpPost]
        public RES Post()
        {
            var className = this.Request.Form["class"];
            var method = this.Request.Form["method"]; 
            var jsonReq = this.Request.Form["jsonReq"];
            var jsonRes = this.Request.Form["jsonRes"];
            var res = RES.FAIL("method参数未匹配");
             
            if ("CreateTask北向持股统计" == method)
            {
                res = DataProcNode.GetInst().CreateTask北向持股统计(jsonReq, jsonRes);
            }
            return res;
        }
    }
}
