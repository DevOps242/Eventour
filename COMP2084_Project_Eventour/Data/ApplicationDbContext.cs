using COMP2084_Project_Eventour.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace COMP2084_Project_Eventour.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // add the models to the migration
    public DbSet<Event> Events { get; set; }
    public DbSet<EventDetail> EventDetails { get; set; }
    public DbSet<EventVenue> EventVenues { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    new public DbSet<User> Users { get; set; }
}

