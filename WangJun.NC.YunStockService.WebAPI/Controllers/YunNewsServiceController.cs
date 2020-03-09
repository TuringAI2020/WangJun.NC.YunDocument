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
    public class YunNewsServiceController : ControllerBase
    { 

        private readonly ILogger<YunNewsServiceController> _logger;

        public YunNewsServiceController(ILogger<YunNewsServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public RES Get(string appId, string categoryId,int pageIndex, int pageSize)
        {
            var res = RES.FAIL("keyName参数未匹配");
            res = YunNewsService.GetInst().QueryList(null);
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
 
            return res;
        }
    }
}
