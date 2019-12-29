using System;
using System.IO;
using System.Linq;
using WangJun.NC.YunDocument.Front;
using WangJun.NC.YunUtils;

namespace WangJun.NC.YunDocument.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            //var res = DB.SaveEntity<LogOperation>(new LogOperation { CreateTime=DateTime.Now, Remark=$"测试{DateTime.Now}", AppId=Guid.NewGuid() });
            //var res = DB.SaveEntity<LogOperation>(new LogOperation { Id=Guid.Parse("C8A1E1B5-799C-4690-9CDF-7FF204F79C10") , UserName="汪俊", CreateTime = DateTime.Now, Remark = $"测试{DateTime.Now}", AppId = Guid.NewGuid() });
            //var res = DB.DeleteEntity<LogOperation>(new LogOperation { Id = Guid.Parse("C8A1E1B5-799C-4690-9CDF-7FF204F79C10"), UserName = "汪俊", CreateTime = DateTime.Now, Remark = $"测试{DateTime.Now}", AppId = Guid.NewGuid() });
            //Console.WriteLine("Hello World!"+res.Result);

            ///REDIS先写数据
            ///DB再同步
            ///
            var files = Directory.GetFiles(@"D:\test").ToList();
            var frontDB = FrontDB.Current;

            for (int k = 0; k < files.Count; k++)
            {
                var p = files[k];
                var file = new FileInfo(p);
                var id = Guid.Parse($"00000000-0000-0000-0000-00000000000{k + 1}");
                frontDB.Save(JSON.ToJson(new Document
                {
                    ItemID= id
                    ,Id = id
                    ,Title = file.Name
                    , Detail = File.ReadAllText(p)
                    , CreateTime=DateTime.Now
                    , Status = (int)ENUM.实体状态.正常
                    , CreatorId = Guid.Empty
                    , Sort = DateTime.Now.Second
                })); 
            }

            var query = frontDB.QueryList(JSON.ToJson(new QueryFilter { Status = "1" }));
            
            //var classFullName = typeof(Front.LogOperation).FullName;
            //for (int k = 0; k < 100; k++)
            //{
            //    var id = Guid.NewGuid();
            //    frontDB.Save<Front.LogOperation>(new Front.LogOperation { ItemID = id, ClassFullName = classFullName, Id = id, CreateTime = DateTime.Now, Remark = $"测试{DateTime.Now}\t{k}", AppId = Guid.NewGuid() });
            //}

            //DB.SyncDB<Front.LogOperation,LogOperation>();
            Console.WriteLine("OK");



            Console.ReadKey();
        }
    }
}
