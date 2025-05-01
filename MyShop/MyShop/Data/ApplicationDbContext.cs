using Microsoft.EntityFrameworkCore;
using MyShop.Data.Entities;
using MyShopClassLibrary.Models;

namespace MyShop.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ApplicationUsers> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<OrderDetail> Orders { get; set; }
    public DbSet<OrderItems> OrderItems { get; set; }
    public DbSet<CartItems> CartItems { get; set; }
    public DbSet<Reviews> Reviews { get; set; }
    public DbSet<CategorySub> Subs { get; set; }
    public DbSet<Stats> Stats { get; set; }

}
