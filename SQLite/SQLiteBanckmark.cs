using BenchmarkDotNet.Attributes;
using Microsoft.EntityFrameworkCore;
using SQLBenchmark.Context;
using SQLBenchmark.Models;
using System;
using System.Security.Cryptography;
using System.Text;
using Z.Dapper.Plus;

namespace SQLBenchmark.SQLite
{
    public class SQLiteBanckmark
    {
        [Params(50000)]
        public int DataCount { get; set; }

        private readonly SQLiteContext _sqliteContext;
        private Post[] _posts;

        public SQLiteBanckmark()
        {
            _sqliteContext = new SQLiteContext();
        }

        [GlobalSetup]
        public void Setup()
        {
            _posts = new Post[DataCount];

            for (int i = 0; i < _posts.Length; i++)
            {
                _posts[i] = new Post
                {
                    Id = i,
                    Account = GenerateString(20),
                    Title = GenerateString(30),
                    Context = GenerateString(100),
                    Read = new Random().Next(0, 2)
                };
            }
            _sqliteContext.Database.Migrate();
        }

        [GlobalCleanup]
        public void Cleanup()
        {
            _posts = null;
            _sqliteContext.Database.EnsureDeleted();
            Console.WriteLine("Cleanup");
        }

        #region Dapper

        [Benchmark]
        public void InsertUseDapper()
        {
            _sqliteContext.Database.GetDbConnection()
                .BulkInsert(_posts);
        }

        [Benchmark]
        public void MergeUseDapper()
        {
            _sqliteContext.Database.GetDbConnection()
                .BulkMerge(_posts);
        }

        #endregion Dapper

        #region Official

        [Benchmark]
        public void InsertUseOfficial()
        {
            var dbConnection = _sqliteContext.Database.GetDbConnection();
            var cmd = dbConnection.CreateCommand();
            var cmdBuilder = new StringBuilder();

            try
            {
                cmdBuilder.AppendLine($"INSERT OR IGNORE INTO post(id,account,title,context,read)");
                cmdBuilder.AppendLine("VALUES ");
                for (int i = 0; i < DataCount; i++)
                {
                    cmdBuilder.Append($"('{_posts[i].Id}', '{_posts[i].Account}'," +
                        $"'{_posts[i].Title}', '{_posts[i].Context}', '{_posts[i].Read}')");
                    cmdBuilder.Append(i == (DataCount - 1) ? string.Empty : ", ");
                }
                cmdBuilder.Append(';');
                cmd.CommandText = cmdBuilder.ToString();
                dbConnection.Open();
                cmd.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                dbConnection.Close();
            }
        }

        [Benchmark]
        public void MergeUseOfficial()
        {
            var dbConnection = _sqliteContext.Database.GetDbConnection();
            var cmd = dbConnection.CreateCommand();
            var cmdBuilder = new StringBuilder();

            try
            {
                cmdBuilder.AppendLine($"INSERT INTO post(id,account,title,context,read)");
                cmdBuilder.AppendLine("VALUES ");
                for (int i = 0; i < DataCount; i++)
                {
                    cmdBuilder.Append($"('{_posts[i].Id}', '{_posts[i].Account}'," +
                        $"'{_posts[i].Title}', '{_posts[i].Context}', '{_posts[i].Read}')");
                    cmdBuilder.Append(i == (DataCount - 1) ? string.Empty : ", ");
                }
                cmdBuilder.AppendLine(string.Empty);
                cmdBuilder.AppendLine("ON CONFLICT(id) DO UPDATE SET");
                cmdBuilder.Append("account=excluded.account,title=excluded.title,context=excluded.context,read=excluded.read;");
                cmd.CommandText = cmdBuilder.ToString();
                dbConnection.Open();
                cmd.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                dbConnection.Close();
            }
        }

        #endregion Official

        private static string GenerateString(int length)
        {
            using var crypto = new RNGCryptoServiceProvider();
            var bits = (length * 6);
            var byteSize = ((bits + 7) / 8);
            var bytesArray = new byte[byteSize];
            crypto.GetBytes(bytesArray);
            return Convert.ToBase64String(bytesArray);
        }
    }
}