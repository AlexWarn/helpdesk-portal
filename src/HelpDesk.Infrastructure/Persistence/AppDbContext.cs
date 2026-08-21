using HelpDesk.Domain.Entities;
using HelpDesk.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace HelpDesk.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");

            entity.HasKey(x => x.Id);

            entity.Property(x => x.Email)
                .HasConversion(
                    email => email.Value,
                    value => Email.Create(value))
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(x => x.Email)
                .IsUnique();

            entity.Property(x => x.PasswordHash)
                .IsRequired();

            entity.ComplexProperty(x => x.FullName, fullName =>
            {
                fullName.Property(x => x.FirstName)
                    .HasColumnName("first_name")
                    .HasMaxLength(100)
                    .IsRequired();

                fullName.Property(x => x.LastName)
                    .HasColumnName("last_name")
                    .HasMaxLength(100)
                    .IsRequired();

                fullName.Property(x => x.MiddleName)
                    .HasColumnName("middle_name")
                    .HasMaxLength(100);
            });

            entity.Property(x => x.Role)
                .IsRequired();

            entity.Property(x => x.IsBlocked)
                .IsRequired();

            entity.Property(x => x.CreatedAtUtc)
                .IsRequired();
        });
    }
}