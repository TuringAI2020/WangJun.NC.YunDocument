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
    public class YunAIServiceController : ControllerBase
    { 

        private readonly ILogger<YunAIServiceController> _logger;

        public YunAIServiceController(ILogger<YunAIServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public RES Get(string token,string method, string req)
        {
            var inst = AI_T.GetInst();
            var res = inst.GetType().GetMethod(method).Invoke(inst, new object[] { req });
            return res as RES;
        }

        [HttpPost]
        public RES Post()
        { 
            var method = this.Request.Form["method"];
            var req = this.Request.Form["req"];
            var inst = AI_T.GetInst();
            var res = inst.GetType().GetMethod(method).Invoke(inst, new object[] { req.ToString() });
            return res as RES;
        }
    }
}
