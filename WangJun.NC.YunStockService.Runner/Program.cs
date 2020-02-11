using System;
///using WangJun.NC.YunStockService.Models;

namespace WangJun.NC.YunStockService.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
 
            new SyncTaskRunner().Run();
            Console.WriteLine("Hello World!");
        }
    }
}
