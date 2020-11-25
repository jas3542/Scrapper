using JobScapper.Objects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JobScaper.DBContext
{
    class DBScraperContext : DbContext
    {
        public DbSet<Job> Jobs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Reading appSettings connection string:
            var builder = new ConfigurationBuilder()
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddJsonFile("appSettings.json", optional: false, reloadOnChange: true);
            var configuration = builder.Build();

            var connectionString = configuration["DBConnection:url"];
            optionsBuilder.UseSqlite(connectionString);
        }

        




    }
}
