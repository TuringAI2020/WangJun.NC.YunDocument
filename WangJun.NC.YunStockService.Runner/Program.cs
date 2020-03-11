using System;
using WangJun.NC.YunUtils;
///using WangJun.NC.YunStockService.Models;

namespace WangJun.NC.YunStockService.Runner
{
    class Program
    {
        static void Main(string[] args)
        {

            //new SyncTaskRunner().Run();
            var res = AI_T.KeywordsExtraction("腾讯云—腾讯倾力打造的云计算品牌，以卓越科技能力助力各行各业数字化转型，为全球客户提供领先的云计算");
            Console.WriteLine("Hello World!");
        }
    }
}
