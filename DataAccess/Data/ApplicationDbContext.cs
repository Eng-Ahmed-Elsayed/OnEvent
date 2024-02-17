using DataAccess.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Event> Events { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Logistics> Logistics { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<RSVP> RSVPs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Roles Config
            modelBuilder.ApplyConfiguration(new RoleConfiguration());

            // Events with Organizer
            modelBuilder.Entity<Event>()
                .HasOne(e => e.Organizer)
                .WithMany(e => e.Events)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event with Invitations
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Invitations)
                .WithOne(e => e.Event)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);


            // Event with Guests
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Guests)
                .WithOne(e => e.Event)
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event with Logistics
            modelBuilder.Entity<Event>()
                .HasMany(e => e.Logistics)
                .WithOne()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Event with RSVPs
            modelBuilder.Entity<Event>()
                .HasMany<RSVP>()
                .WithOne()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Guest with RSVP
            modelBuilder.Entity<Guest>()
                .HasOne(e => e.RSVP)
                .WithOne()
                .HasForeignKey<RSVP>(e => e.GuestId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notifications with Event
            modelBuilder.Entity<Notification>()
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // Notifications with User
            modelBuilder.Entity<Notification>()
                .HasOne<User>()
                .WithMany(e => e.Notifications)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // EmailModels with Event
            modelBuilder.Entity<EmailModel>()
                .HasOne<Event>()
                .WithMany()
                .HasForeignKey(e => e.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            // EmailModels with User
            modelBuilder.Entity<EmailModel>()
                .HasOne(e => e.User)
                .WithMany(e => e.EmailModels)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
