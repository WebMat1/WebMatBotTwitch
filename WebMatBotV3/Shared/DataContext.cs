using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMatBotV3.Shared.Entity;

namespace WebMatBotV3.Shared
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Scores> Scores {get;set;}
        public DbSet<Points> Points {get;set;}
        public DbSet<Seasons> Seasons {get;set;}
        public DbSet<Records> Records {get;set;}
        public DbSet<Resources> Resources {get;set;}
        public DbSet<Invitations> Invitations {get;set;}
        public DbSet<Balls> Balls { get; set; }
        public DbSet<Inventories> Inventories { get; set; }
        public DbSet<SubscribersResources> SubscribersResources { get; set; }
        public DbSet<Usernames> Usernames { get; set; }
    }
}
