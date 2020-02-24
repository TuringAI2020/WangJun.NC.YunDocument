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
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<YunStock2ServiceController> _logger;

        public YunStock2ServiceController(ILogger<YunStock2ServiceController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public RES Get(string taskId)
        {
            var res =  DataProcNode.GetInst().GetTask(taskId);
            return res;
        }

        [HttpPost]
        public RES Post()
        {
            var method = this.Request.Form["method"];
            var taskId = this.Request.Form["taskId"];
            var jsonReq = this.Request.Form["jsonReq"];
            var jsonRes = this.Request.Form["jsonRes"];
            var res = DataProcNode.GetInst().SaveBXCGMX(taskId,jsonReq, jsonRes);
            return res;
        }
    }
}
