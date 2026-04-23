using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneMillionCopy.Leads.Domain.Entities;

namespace OneMillionCopy.Leads.Infrastructure.Persistence.Configurations;

public sealed class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("Leads");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nombre)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.Telefono)
            .HasMaxLength(50);

        builder.Property(x => x.ProductoInteres)
            .HasMaxLength(200);

        builder.Property(x => x.Presupuesto)
            .HasPrecision(18, 2);

        builder.Property(x => x.Fuente)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .IsRequired();

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("IX_Leads_Email");

        builder.HasQueryFilter(x => x.DeletedAtUtc == null);
    }
}
