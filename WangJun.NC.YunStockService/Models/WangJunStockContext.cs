using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace WangJun.NC.YunStockService.Models
{
    public partial class WangJunStockContext : DbContext
    {
        public WangJunStockContext()
        {
        }

        public WangJunStockContext(DbContextOptions<WangJunStockContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<Conception> Conception { get; set; }
        public virtual DbSet<Hxtc> Hxtc { get; set; }
        public virtual DbSet<Jgdylb> Jgdylb { get; set; }
        public virtual DbSet<Keywords> Keywords { get; set; }
        public virtual DbSet<RelationConception> RelationConception { get; set; }
        public virtual DbSet<ShortNews> ShortNews { get; set; }
        public virtual DbSet<StockCode> StockCode { get; set; }
        public virtual DbSet<北向代码> 北向代码 { get; set; }
        public virtual DbSet<北向成交明细> 北向成交明细 { get; set; }
        public virtual DbSet<北向持股明细> 北向持股明细 { get; set; }
        public virtual DbSet<北向持股统计> 北向持股统计 { get; set; }
        public virtual DbSet<北向机构持股明细> 北向机构持股明细 { get; set; }
        public virtual DbSet<所有机构> 所有机构 { get; set; }
        public virtual DbSet<沪深股通机构> 沪深股通机构 { get; set; }
        public virtual DbSet<融资融券> 融资融券 { get; set; }
        public virtual DbSet<财务主要指标> 财务主要指标 { get; set; }
        public virtual DbSet<资金流向> 资金流向 { get; set; }
        public virtual DbSet<重要代码> 重要代码 { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=.\\SQL2017;Initial Catalog=WangJunStock;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Article>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CategoryCode).HasMaxLength(50);

                entity.Property(e => e.CategoryName).HasMaxLength(50);

                entity.Property(e => e.Content).IsRequired();

                entity.Property(e => e.DataSource)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Href)
                    .IsRequired()
                    .HasMaxLength(2048);

                entity.Property(e => e.PublishTime).HasColumnType("datetime");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Conception>(entity =>
            {
                entity.HasKey(e => e.Name);

                entity.Property(e => e.Name)
                    .HasMaxLength(20)
                    .IsFixedLength();
            });

            modelBuilder.Entity<Hxtc>(entity =>
            {
                entity.ToTable("HXTC");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.Detail).IsRequired();

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<Jgdylb>(entity =>
            {
                entity.ToTable("JGDYLB");

                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.公告日期)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.公告日期tag).HasColumnName("公告日期Tag");

                entity.Property(e => e.接待人员)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.接待地点)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.接待方式)
                    .IsRequired()
                    .HasMaxLength(500);

                entity.Property(e => e.接待日期)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.接待日期tag).HasColumnName("接待日期Tag");
            });

            modelBuilder.Entity<Keywords>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Keyword)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<RelationConception>(entity =>
            {
                entity.HasKey(e => new { e.Code, e.Conception });

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.Conception).HasMaxLength(50);

                entity.Property(e => e.Score)
                    .HasColumnType("numeric(18, 2)")
                    .HasDefaultValueSql("((0))");
            });

            modelBuilder.Entity<ShortNews>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasMaxLength(2048);

                entity.Property(e => e.Href).HasMaxLength(2048);

                entity.Property(e => e.PublishTime).HasColumnType("datetime");

                entity.Property(e => e.Source)
                    .IsRequired()
                    .HasMaxLength(20)
                    .IsFixedLength();
            });

            modelBuilder.Entity<StockCode>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsFixedLength();

                entity.Property(e => e.Prefix)
                    .HasMaxLength(2)
                    .IsFixedLength();
            });

            modelBuilder.Entity<北向代码>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(8)
                    .IsFixedLength();
            });

            modelBuilder.Entity<北向成交明细>(entity =>
            {
                entity.HasKey(e => new { e.Code, e.日期tag })
                    .HasName("PK_北向成交");

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.日期tag).HasColumnName("日期Tag");

                entity.Property(e => e.收盘价).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.沪深股通买入金额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.沪深股通净买额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.沪深股通卖出金额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.沪深股通成交金额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.涨跌幅).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<北向持股明细>(entity =>
            {
                entity.HasKey(e => new { e.Code, e.持股日期tag, e.机构名称 });

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.持股日期tag).HasColumnName("持股日期Tag");

                entity.Property(e => e.机构名称).HasMaxLength(200);

                entity.Property(e => e.当日收盘价).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.当日涨跌幅).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股市值).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股市值变化10日).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股市值变化1日).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股市值变化5日).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股数量).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股数量占a股百分比)
                    .HasColumnName("持股数量占A股百分比")
                    .HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<北向持股统计>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.JgCode)
                    .IsRequired()
                    .HasColumnName("jgCode")
                    .HasMaxLength(200);

                entity.Property(e => e.持股市值).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股市值变化10日).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股市值变化1日).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股市值变化5日).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股日期)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.持股日期tag).HasColumnName("持股日期Tag");

                entity.Property(e => e.机构名称)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<北向机构持股明细>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.JgCode)
                    .IsRequired()
                    .HasColumnName("jgCode")
                    .HasMaxLength(200);

                entity.Property(e => e.当日收盘价).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.当日涨跌幅).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股市值).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股市值变化10日).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股市值变化1日).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股市值变化5日).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股数量).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股数量占a股百分比)
                    .HasColumnName("持股数量占A股百分比")
                    .HasColumnType("numeric(18, 2)");

                entity.Property(e => e.持股日期tag).HasColumnName("持股日期Tag");

                entity.Property(e => e.机构名称)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<所有机构>(entity =>
            {
                entity.HasKey(e => new { e.JgCode, e.JgName });

                entity.Property(e => e.JgCode)
                    .HasColumnName("jgCode")
                    .HasMaxLength(100);

                entity.Property(e => e.JgName)
                    .HasColumnName("jgName")
                    .HasMaxLength(100);

                entity.Property(e => e.Href)
                    .IsRequired()
                    .HasMaxLength(1024);

                entity.Property(e => e.Tag).HasMaxLength(500);
            });

            modelBuilder.Entity<沪深股通机构>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.机构名称)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<融资融券>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.交易日期)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.交易日期tag).HasColumnName("交易日期Tag");

                entity.Property(e => e.收盘价).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.涨跌幅).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融券余量).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融券余额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融券偿还量).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融券净卖出).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融券卖出量).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融资买入额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融资余额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融资余额占流通市值比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融资偿还额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融资净买入).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融资融券余额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.融资融券余额差值).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<财务主要指标>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.净利率).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.加权净资产收益率).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.基本每股收益)
                    .HasColumnType("numeric(18, 4)")
                    .HasComment("基本每股收益(元)");

                entity.Property(e => e.存货周转天数).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.实际税率).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.应收账款周转天数).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.归属净利润)
                    .HasColumnType("numeric(18, 2)")
                    .HasComment("归属净利润(元)");

                entity.Property(e => e.归属净利润同比增长)
                    .HasColumnType("numeric(18, 2)")
                    .HasComment("归属净利润同比增长(%)");

                entity.Property(e => e.归属净利润滚动环比增长)
                    .HasColumnType("numeric(18, 2)")
                    .HasComment("归属净利润滚动环比增长(%)");

                entity.Property(e => e.总资产周转率).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.扣非净利润)
                    .HasColumnType("numeric(18, 2)")
                    .HasComment("扣非净利润(元)");

                entity.Property(e => e.扣非净利润同比增长)
                    .HasColumnType("numeric(18, 2)")
                    .HasComment("扣非净利润同比增长(%)");

                entity.Property(e => e.扣非净利润滚动环比增长).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.扣非每股收益)
                    .HasColumnType("numeric(18, 4)")
                    .HasComment("扣非每股收益(元)");

                entity.Property(e => e.摊薄净资产收益率).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.摊薄总资产收益率).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.每股公积金)
                    .HasColumnType("numeric(18, 4)")
                    .HasComment("每股公积金(元)");

                entity.Property(e => e.每股未分配利润)
                    .HasColumnType("numeric(18, 4)")
                    .HasComment("每股未分配利润(元)");

                entity.Property(e => e.每股经营现金流)
                    .HasColumnType("numeric(18, 4)")
                    .HasComment("每股经营现金流(元)");

                entity.Property(e => e.毛利润)
                    .HasColumnType("numeric(18, 2)")
                    .HasComment("毛利润(元)");

                entity.Property(e => e.毛利率).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.流动比率).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.流动负债总负债).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.稀释每股收益)
                    .HasColumnType("numeric(18, 4)")
                    .HasComment("稀释每股收益(元)");

                entity.Property(e => e.经营现金流营业收入).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.营业总收入)
                    .HasColumnType("numeric(18, 2)")
                    .HasComment("营业总收入(元)");

                entity.Property(e => e.营业总收入同比增长)
                    .HasColumnType("numeric(18, 2)")
                    .HasComment("营业总收入同比增长(%)");

                entity.Property(e => e.营业总收入滚动环比增长)
                    .HasColumnType("numeric(18, 2)")
                    .HasComment("营业总收入滚动环比增长(%)");

                entity.Property(e => e.资产负债率).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.速动比率).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.销售现金流营业收入).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.预收款营业收入).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<资金流向>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Code)
                    .IsRequired()
                    .HasMaxLength(6)
                    .IsFixedLength();

                entity.Property(e => e.中单净流入净占比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.中单净流入净额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.主力净流入净占比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.主力净流入净额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.大单净流入净占比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.大单净流入净额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.小单净流入净占比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.小单净流入净额).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.收盘价).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.日期)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsFixedLength();

                entity.Property(e => e.日期tag).HasColumnName("日期Tag");

                entity.Property(e => e.涨跌幅).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.超大单净流入净占比).HasColumnType("numeric(18, 2)");

                entity.Property(e => e.超大单净流入净额).HasColumnType("numeric(18, 2)");
            });

            modelBuilder.Entity<重要代码>(entity =>
            {
                entity.HasKey(e => e.Code);

                entity.Property(e => e.Code)
                    .HasMaxLength(6)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
