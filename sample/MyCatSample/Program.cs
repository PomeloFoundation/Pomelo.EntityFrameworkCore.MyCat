using System;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MyCatSample
{
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
                .UseMyCat("server=192.168.0.102;port=8066;uid=test;pwd=test;database=blog")
                .UseDataNode("192.168.0.102", "mycatblog1", "root", "19931101")
                .UseDataNode("192.168.0.102", "mycatblog2", "root", "19931101");
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var DB = new SampleContext();
            DB.Database.EnsureCreated();
            for (var i = 0; i < 50; i++)
                DB.Blogs.Add(new Blog { Title = "Hello", Time = DateTime.Now, Content = "MyCat" });
            DB.SaveChanges();
            Console.WriteLine(DB.Blogs.Count());
            Console.Read();
        }
    }
}
