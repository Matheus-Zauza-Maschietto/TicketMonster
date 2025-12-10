using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TicketMonster.Domain;

namespace TicketMonster.Repositories;

public class ApplicationDbContext : IdentityDbContext<UserEntity>
{
    public override DbSet<UserEntity> Users { get; set; }
    public DbSet<ShowEntity> Shows { get; set; }
    public DbSet<TicketEntity> Tickets { get; set; }
    public DbSet<PaymentEntity> Payments { get; set; }


    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("Users");
            entity.HasMany(s => s.Tickets)
                .WithOne(t => t.UserOwner)
                .HasForeignKey(t => t.UserOwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(s => s.CreatedShows)
                .WithOne(s => s.UserCreator)
                .HasForeignKey(s => s.UserCreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<ShowEntity>(entity =>
        {
            entity.ToTable("Shows");

            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id)
                .ValueGeneratedOnAdd();

            entity.Property(s => s.Title)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(s => s.Singer)
                .IsRequired()
                .HasMaxLength(150);

            entity.Property(s => s.TicketPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.Property(s => s.PresentationDate)
                .IsRequired();

            entity.Property(s => s.MaxTicketQuantity)
                .IsRequired();

            entity.HasOne(s => s.UserCreator)
                .WithMany(s => s.CreatedShows)
                .HasForeignKey(s => s.UserCreatorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<TicketEntity>(entity =>
        {
            entity.ToTable("Tickets");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id)
                .ValueGeneratedOnAdd();

            entity.Property(t => t.Code)
                .IsRequired();

            entity.HasOne(t => t.UserOwner)
                .WithMany(t => t.Tickets)
                .HasForeignKey(t => t.UserOwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Show)
                .WithMany(t => t.Tickets)
                .HasForeignKey(t => t.ShowId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(t => t.Payment)
                .WithOne(p => p.Ticket)
                .HasForeignKey<TicketEntity>(t => t.PaymentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<PaymentEntity>(entity =>
        {
            entity.ToTable("Payments");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .ValueGeneratedOnAdd();

            entity.Ignore(p => p.Value);

            entity.Property(p => p.ValueInCents)
                .IsRequired()
                .HasPrecision(18, 2);

            entity.Property(p => p.CreationDate)
                .IsRequired();
            
            entity.Property(p => p.ExpirationDate)
                .IsRequired();

            entity.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            entity.Property(p => p.ClientSecret)
                .IsRequired(false);
        });
    }
}
