using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Model.pi
{
    public partial class piDBContext : DbContext
    {
        public piDBContext()
        {
        }

        public piDBContext(DbContextOptions<piDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<tb_pi_test> tb_pi_tests { get; set; }
        public virtual DbSet<test> tests { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=172.30.199.20;database=pi;user id=root;password=Docimax@123", Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.31-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasCharSet("utf8")
                .UseCollation("utf8_general_ci");

            modelBuilder.Entity<tb_pi_test>(entity =>
            {
                entity.HasComment("树莓派测试");

                entity.Property(e => e.id).HasComment("ID 主键");

                entity.Property(e => e.create_time).HasComment("创建时间");

                entity.Property(e => e.create_user).HasComment("创建人");

                entity.Property(e => e.is_delete).HasComment("是否删除");

                entity.Property(e => e.is_success).HasComment("是否成功");

                entity.Property(e => e.order_number).HasComment("排序字段");

                entity.Property(e => e.update_time).HasComment("修改时间");

                entity.Property(e => e.update_user).HasComment("修改人");
            });

            modelBuilder.Entity<test>(entity =>
            {
                entity.Property(e => e.id).HasComment("ID 主键");

                entity.Property(e => e.create_time).HasComment("创建时间");

                entity.Property(e => e.create_user).HasComment("创建人");

                entity.Property(e => e.is_delete).HasComment("是否删除");

                entity.Property(e => e.is_success).HasComment("是否成功");

                entity.Property(e => e.order_number).HasComment("排序字段");

                entity.Property(e => e.update_time).HasComment("修改时间");

                entity.Property(e => e.update_user).HasComment("修改人");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}