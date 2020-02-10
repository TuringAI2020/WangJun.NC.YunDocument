using System;
using WangJun.NC.YunStockService.Models;

namespace WangJun.NC.YunStockService.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            //BackDB.Current.Save<StockCode>(new StockCode { Code = "Test", Name = "Test" });
            new SyncTaskRunner().Run();
            Console.WriteLine("Hello World!");
        }
    }
}
