using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Resort.Domain.Bookings;
using Resort.Domain.Places;
using Resort.Domain.Users;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Resort.Infrastructure.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Place> Places => Set<Place>();
    public DbSet<Feature> Features => Set<Feature>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ---------- Place ----------
        modelBuilder.Entity<Place>(entity =>
        {
            entity.ToTable("Places");
            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(DbConst.Place.MaxNameLength);

            entity.Property(p => p.City)
                .IsRequired()
                .HasMaxLength(DbConst.Place.MaxCityLength);

            entity.Property(p => p.Region)
                .IsRequired()
                .HasMaxLength(DbConst.Place.MaxRegionLength);

            entity.Property(p => p.Address)
                .IsRequired()
                .HasMaxLength(DbConst.Place.MaxAddressLength);

            entity.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(DbConst.Place.MaxDescriptionLength);

            entity.Property(p => p.GoogleMapUrl)
                .HasMaxLength(DbConst.Place.MaxGoogleMapUrlLength);

            entity.Property(p => p.PricePerNight)
                .HasColumnType($"decimal({DbConst.Booking.PricePrecision},{DbConst.Booking.PriceScale})");

            // ✅ ImagePaths (string[]) => JSON في NVARCHAR(MAX)
            var stringArrayToJsonConverter = new ValueConverter<string[], string>(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => string.IsNullOrWhiteSpace(v)
                    ? Array.Empty<string>()
                    : JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null) ?? Array.Empty<string>());

            var stringArrayComparer = new ValueComparer<string[]>(
                (a, b) => (a ?? Array.Empty<string>()).SequenceEqual(b ?? Array.Empty<string>()),
                a => (a ?? Array.Empty<string>()).Aggregate(0, (hash, item) => HashCode.Combine(hash, item.GetHashCode())),
                a => (a ?? Array.Empty<string>()).ToArray()
            );

            var imagePathsProperty = entity.Property(p => p.ImagePaths);

            imagePathsProperty
                .HasConversion(stringArrayToJsonConverter)
                .HasColumnType("nvarchar(max)");

            imagePathsProperty.Metadata.SetValueComparer(stringArrayComparer);

        });

        // ---------- Feature ----------
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.ToTable("Features");
            entity.HasKey(f => f.Id);

            entity.Property(f => f.Name)
                .IsRequired()
                .HasMaxLength(DbConst.Feature.MaxNameLength);

            entity.Property(f => f.IconKey)
                .IsRequired()
                .HasMaxLength(DbConst.Feature.MaxIconKeyLength);
        });

        modelBuilder.Entity<Place>()
            .HasMany(p => p.Features)
            .WithMany(f => f.Places)
            .UsingEntity(j => j.ToTable("PlaceFeatures"));

        // ---------- Booking ----------
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.ToTable("Bookings");
            entity.HasKey(b => b.Id);

            entity.Property(b => b.TotalPrice)
                .HasColumnType($"decimal({DbConst.Booking.PricePrecision},{DbConst.Booking.PriceScale})");

            entity.Property(b => b.PaymentFee)
                .HasColumnType($"decimal({DbConst.Booking.PricePrecision},{DbConst.Booking.PriceScale})");

            entity.Property(b => b.PaymentMethod)
                .IsRequired()
                .HasMaxLength(40);

            entity.Property(b => b.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(40);

            entity.Property(b => b.CreatedAtUtc)
                .HasDefaultValueSql("GETUTCDATE()"); // SQL Server

            entity.Property(b => b.Status)
                .HasConversion<int>();

            entity.HasOne(b => b.Place)
                .WithMany()
                .HasForeignKey(b => b.PlaceId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.Property(b => b.UserId)
                .HasMaxLength(DbConst.Booking.MaxUserIdLength);
        });


        // ---------- User ----------
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(x => x.Id);

            e.Property(x => x.FullName)
                .HasMaxLength(120)
                .IsRequired();

            e.Property(x => x.Email)
                .HasMaxLength(256)
                .IsRequired();

            e.HasIndex(x => x.Email).IsUnique();

            e.Property(x => x.PasswordHash).IsRequired();
        });


    }
}
