using MehranBot.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace MehranBot.Models.Context;

public class MehranBotDbContext : DbContext
{
    public MehranBotDbContext(DbContextOptions options) : base(options)
    {

    }



    public DbSet<User> Users { get; set; }
    public DbSet<Ads> Ads { get; set; }
    public DbSet<Category> Category { get; set; }
    public DbSet<Setting> Setting { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<UserPayment> UserPayment { get; set; }

}
