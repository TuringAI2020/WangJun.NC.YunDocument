﻿using System;
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

        public virtual DbSet<Conception> Conception { get; set; }
        public virtual DbSet<Hxtc> Hxtc { get; set; }
        public virtual DbSet<RelationConception> RelationConception { get; set; }
        public virtual DbSet<StockCode> StockCode { get; set; }
        public virtual DbSet<财务主要指标> 财务主要指标 { get; set; }

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

            modelBuilder.Entity<财务主要指标>(entity =>
            {
                entity.HasKey(e => new { e.Code, e.DateTag });

                entity.Property(e => e.Code)
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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
