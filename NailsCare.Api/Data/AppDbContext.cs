using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using NailsCare.Api.Models;

namespace NailsCare.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<GalleryImage> GalleryImages => Set<GalleryImage>();
    }
}
