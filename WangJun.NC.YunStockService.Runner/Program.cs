using System;
using WangJun.NC.YunUtils;
///using WangJun.NC.YunStockService.Models;

namespace WangJun.NC.YunStockService.Runner
{
    class Program
    {
        static void Main(string[] args)
        {

            SyncTaskRunner.GetInst().Run(); 
            Console.WriteLine("OK");
        }
    }
}
