using Microsoft.EntityFrameworkCore;

namespace SQLBenchmark.Context
{
    public class SQLiteContext : ContextBase
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlite("Data Source=./sqlite.db");
        }
    }
}