using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WangJun.NC.YunDocument.Models;

namespace WangJun.NC.YunDocument
{
    public partial class BackDB : WangJun.NC.YunUtils.DB
    {
        private static  BackDB backDB = null;
        public static BackDB Current {
            get
            {
                if (backDB == null)
                {
                    backDB = new BackDB();
                }
                return backDB;
            }
        }

        public BackDB()
        {
        }

        public BackDB(DbContextOptions<WangJun.NC.YunUtils.DB> options)
            : base(options)
        {
        } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                 optionsBuilder.UseSqlServer(Config.DBConnection);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AppId).HasColumnName("AppID").IsRequired();

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.ParentId).HasColumnName("ParentID");

                entity.Property(e => e.PathIdarray).HasColumnName("PathIDArray");

                entity.Property(e => e.RootId).HasColumnName("RootID");
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.CreatorId).HasColumnName("CreatorID");

                entity.Property(e => e.DataSourceName).HasMaxLength(50);

                entity.Property(e => e.EditorId).HasColumnName("EditorID");

                entity.Property(e => e.FirstVersionId).HasColumnName("FirstVersionID");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.Summary).IsRequired();

                entity.Property(e => e.TagGroupId).HasColumnName("TagGroupID");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(512);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.Version).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<LogOperation>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasColumnName("ID")
                    .ValueGeneratedNever();

                entity.Property(e => e.AppId).HasColumnName("AppID");

                entity.Property(e => e.BehaviorTypeName).HasMaxLength(50);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.ResultStatusName) 
                    .HasMaxLength(50);

                entity.Property(e => e.TargetId).HasColumnName("TargetID");

                entity.Property(e => e.TargetTypeName).HasMaxLength(50);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserName).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
