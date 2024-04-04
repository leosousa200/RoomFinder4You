using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoomFinder4You.Models;

namespace RoomFinder4You.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public DbSet<Ad> Ads {get;set;}
    public DbSet<Room> Rooms {get;set;}
    public DbSet<Location> Locations{get;set;}
    public DbSet<Feature> Features {get;set;}
    public DbSet<FeatureType> FeatureTypes {get;set;}
    public DbSet<AdStatus> AdsStatus {get;set;}
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
}
