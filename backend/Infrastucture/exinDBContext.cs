﻿using backend.Domain.Cores;
using Microsoft.EntityFrameworkCore;
namespace backend.Infrastucture
{
    public class exinDBContext:DbContext
    {
        protected readonly IConfiguration Configuration;
        public exinDBContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("Data Source=/C:/Users/PCSAM/Desktop/MyExin. db;Version=3"));
        }
        //public DbSet<User> Users { get; set; }
    }
}
