using HotelSearch.API.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelSearch.API.Database.Configurations;

public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
{
    public void Configure(EntityTypeBuilder<Hotel> builder)
    {
        builder.HasKey(e => e.Id);
        
        builder.HasQueryFilter(x => !x.IsDeleted);
        
        builder.Property(e => e.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(e => e.Price)
            .HasPrecision(18, 2);

        builder.Property(e => e.Latitude)
            .HasPrecision(9, 6);

        builder.Property(e => e.Longitude)
            .HasPrecision(9, 6);

        builder.ToTable("Hotels");
    }
}