using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Project1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Project1.Data
{
    public class ArtsContext : DbContext
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        //Property to hold the UserName value
        public string UserName
        {
            get; private set;
        }

        public ArtsContext(DbContextOptions<ArtsContext> options, IHttpContextAccessor httpContextAccessor)
                : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            UserName = _httpContextAccessor.HttpContext?.User.Identity.Name;
            //UserName = (UserName == null) ? "Unknown" : UserName;
            UserName = UserName ?? "Unknown";
        }

        public ArtsContext(DbContextOptions<ArtsContext> options)
            : base(options)
        {
            UserName = "SeedData";
        }
        public DbSet<Artwork> Artworks { get; set; }
        public DbSet<ArtType> ArtTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //Add a unique index to the OHIP Number
            modelBuilder.Entity<Artwork>()
            .HasIndex(p => new { p.Name, p.Completed, p.ArtTypeID})
            .IsUnique();

            modelBuilder.Entity<ArtType>()
                .HasMany(p => p.Artworks)
                .WithOne(d => d.ArtType)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void OnBeforeSaving()
        {
            var entries = ChangeTracker.Entries();
            foreach (var entry in entries)
            {
                if (entry.Entity is IAuditable trackable)
                {
                    var now = DateTime.UtcNow;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;

                        case EntityState.Added:
                            trackable.CreatedOn = now;
                            trackable.CreatedBy = UserName;
                            trackable.UpdatedOn = now;
                            trackable.UpdatedBy = UserName;
                            break;
                    }
                }
            }
        }
    }
}
