using Microsoft.EntityFrameworkCore;
using SQLBenchmark.Models;

namespace SQLBenchmark.Context
{
    public class ContextBase : DbContext
    {
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            CreateBlog(modelBuilder);
        }

        private static void CreateBlog(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>(entity =>
            {
                entity.ToTable("post");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Id)
                      .HasColumnName("id");

                entity.Property(e => e.Account)
                      .HasColumnName("account");

                entity.Property(e => e.Title)
                      .HasColumnName("title");

                entity.Property(e => e.Context)
                      .HasColumnName("context");

                entity.Property(e => e.Read)
                      .HasColumnName("read");
            });
        }
    }
}