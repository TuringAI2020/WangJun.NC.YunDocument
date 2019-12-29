using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using WangJun.NC.YunDocument.Models;

namespace WangJun.NC.YunDocument
{
    public partial class WangJunDocumentContext : DbContext
    {
        public WangJunDocumentContext()
        {
        }

        public WangJunDocumentContext(DbContextOptions<WangJunDocumentContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Document> Document { get; set; }
        public virtual DbSet<LogOperation> LogOperation { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
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

                entity.Property(e => e.AppId).HasColumnName("AppID");

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
