﻿using System;
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
            //var files = Directory.GetFiles(@"D:\test").ToList();
            //var frontDB = FrontDB.Current;

            //for (int k = 0; k < files.Count; k++)
            //{
            //    var p = files[k];
            //    var file = new FileInfo(p);
            //    var id = Guid.Parse($"{k+1}0000000-0000-0000-0000-00000000000{k + 1}");
            //    frontDB.Save<Document>(JSON.ToJson(new Document
            //    {
            //        ItemID = id
            //        ,
            //        Id = id
            //        ,
            //        Title = file.Name
            //        ,
            //        Detail = File.ReadAllText(p)
            //        ,Summary= File.ReadAllText(p)
            //        ,
            //        CreateTime = DateTime.Now
            //        , UpdateTime=DateTime.Now
            //        ,
            //        Status = (int)ENUM.实体状态.正常
            //        ,
            //        CreatorId = Guid.Empty
            //        ,
            //        Sort = DateTime.Now.Second
            //        ,
            //        GroupId = id
            //    }));
            //}

            //var query1 = frontDB.QueryList(JSON.ToJson(new QueryFilter { Status = "1" }));
            //var query2 = frontDB.QueryList(JSON.ToJson(new QueryFilter { Keywords = "苹果" }));
            //var query3 = frontDB.QueryList(JSON.ToJson(new QueryFilter { GroupId = "00000000-0000-0000-0000-000000000003" }));
            // var query4 = frontDB.QueryList(JSON.ToJson(new QueryFilter { CreatorId = "00000000-0000-0000-0000-000000000000" }));


            //var classFullName = typeof(Front.LogOperation).FullName;
            //for (int k = 0; k < 100; k++)
            //{
            //    var id = Guid.NewGuid();
            //    frontDB.Save<Front.LogOperation>(new Front.LogOperation { ItemID = id, ClassFullName = classFullName, Id = id, CreateTime = DateTime.Now, Remark = $"测试{DateTime.Now}\t{k}", AppId = Guid.NewGuid() });
            //}

            //DB.SyncDB<Front.LogOperation,LogOperation>();
            //var id = Guid.Parse($"00000000-0000-0000-0000-000000000002");
            //frontDB.Save(JSON.ToJson(new Document
            //{
            //    ItemID = id
            //    ,
            //    Id = id
            //    ,
            //    Title = "更新标题"
            //    ,
            //    Detail = "更新内容"
            //    ,
            //    CreateTime = DateTime.Now
            //    ,
            //    Status = (int)ENUM.实体状态.正常
            //    ,
            //    CreatorId = Guid.Empty
            //    ,
            //    Sort = DateTime.Now.Second
            //    ,
            //    GroupId = id
            //}));

            //for (int k = 0; k < files.Count; k++)
            //{
            //    var p = files[k];
            //    var file = new FileInfo(p);
            //    var id = Guid.Parse($"{k + 1}0000000-0000-0000-0000-00000000000{k + 1}");
            //    frontDB.Save<Document>(JSON.ToJson(new Document
            //    {
            //        ItemID = id
            //        ,
            //        Id = id
            //        ,
            //        Title = "更新" + file.Name
            //        ,
            //        Detail = "更新"+File.ReadAllText(p)
            //        ,
            //        Summary = "更新" + File.ReadAllText(p)
            //        ,
            //        CreateTime = DateTime.Now
            //        ,
            //        UpdateTime = DateTime.Now
            //        ,
            //        Status = (int)ENUM.实体状态.正常
            //        ,
            //        CreatorId = Guid.Empty
            //        ,
            //        Sort = DateTime.Now.Second
            //        ,
            //        GroupId = id
            //    }));
            //}

            //for (int k = 0; k < files.Count; k++)
            //{
            //    var p = files[k];
            //    var file = new FileInfo(p);
            //    var id = Guid.Parse($"00000000-0000-0000-0000-00000000000{k + 1}");
            //    frontDB.Remove<Document>(JSON.ToJson(new Document
            //    {
            //        ItemID = id
            //        ,
            //        Id = id
            //        ,
            //        Title = "更新" + file.Name
            //        ,
            //        Detail = "更新" + File.ReadAllText(p)
            //        ,
            //        Summary = "更新" + File.ReadAllText(p)
            //        ,
            //        CreateTime = DateTime.Now
            //        ,
            //        UpdateTime = DateTime.Now
            //        ,
            //        Status = (int)ENUM.实体状态.正常
            //        ,
            //        CreatorId = Guid.Empty
            //        ,
            //        Sort = DateTime.Now.Second
            //        ,
            //        GroupId = id
            //    }));
            //}

            //var id = Guid.Parse($"00000000-0000-0000-0000-000000000002");
            //frontDB.Remove(JSON.ToJson(new Document
            //{
            //    ItemID = id
            //    ,
            //    Id = id 
            //}));
            //new SyncTaskRunner().SyncToBackDB<Front.Document, Models.Document>((oldVal,newVal)=> {
            //    oldVal.Title = newVal.Title;
            //    oldVal.Detail = newVal.Detail;
            //    oldVal.Summary = newVal.Summary;
            //    oldVal.UpdateTime = DateTime.Now;
            //});

            #region 目录
            var rootId = Guid.Parse($"10000000-0000-0000-0000-000000000001");
            CategoryFrontDB.Current.CreateRootNode<Category>(JSON.ToJson(new Category
            {
                AppId = rootId,
                CreateTime = DateTime.Now,
                Name = "根目录",
                Id = rootId,
                ItemID = rootId,
                ParentId = Guid.Empty,
                RootId = Guid.Empty,
                Status = (int)ENUM.实体状态.正常
            }));

            var sub1Id = Guid.Parse($"20000000-0000-0000-0000-000000000001");
            CategoryFrontDB.Current.CreateSubNode<Category>(JSON.ToJson(new Category
            {
                AppId = rootId,
                CreateTime = DateTime.Now,
                Name = "一级子目录",
                Id = sub1Id,
                ItemID = sub1Id,
                ParentId = rootId,
                RootId = rootId,
                Status = (int)ENUM.实体状态.正常
            }));

            var sub2Id = Guid.Parse($"30000000-0000-0000-0000-000000000001");
            CategoryFrontDB.Current.CreateSubNode<Category>(JSON.ToJson(new Category
            {
                AppId = rootId,
                CreateTime = DateTime.Now,
                Name = "二级子目录修改",
                Id = sub2Id,
                ItemID = sub2Id,
                ParentId = sub1Id,
                RootId = rootId,
                Status = (int)ENUM.实体状态.正常
            }));

            CategoryFrontDB.Current.RemoveNode<Category>(JSON.ToJson(new Category
            {
                ItemID = sub2Id
            }));
            #endregion

















            Console.WriteLine("OK");



            Console.ReadKey();
        }
    }
}
