using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WangJun.NC.YunDocument.Front;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunDocument.WebAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentController : ControllerBase
    {
 
        private readonly ILogger<DocumentController> _logger;

        public DocumentController(ILogger<DocumentController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public RES Get()
        {
            try {
                return RES.FAIL();
            }
            catch (Exception ex) {
                return RES.FAIL();
            }
        }
    }
}
