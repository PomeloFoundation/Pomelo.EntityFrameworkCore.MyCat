using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyCatSample
{
    [DataNode("dn0, dn1")]
    [Table("xxxx")]
    public class Blog
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime Time { get; set; }

        public JsonObject<List<string>> Tags { get; set; }
    }

    public class SampleContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder
                .UseMyCat("server=192.168.0.102;port=8066;uid=root;pwd=19931101;database=blog")
                .UseDataNode(x =>
                {
                    x.Master = new MyCatDatabaseHost { Host = "192.168.0.102", Database = "mycatblog", Username = "root", Password = "19931101" };
                })
                .UseDataNode(x =>
                {
                    x.Master = new MyCatDatabaseHost { Host = "192.168.0.102", Database = "mycatblog2", Username = "root", Password = "19931101" };
                });
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var DB = new SampleContext();
            DB.Database.EnsureCreated();
            for (var i = 0; i < 1000; i++)
                DB.Blogs.Add(new Blog { Title = "Hello", Time = DateTime.Now, Content = "shabi" });
            DB.SaveChanges();
        }
    }
}
