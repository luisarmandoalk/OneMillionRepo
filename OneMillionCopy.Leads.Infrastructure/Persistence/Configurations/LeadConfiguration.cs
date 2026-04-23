using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OneMillionCopy.Leads.Domain.Entities;

namespace OneMillionCopy.Leads.Infrastructure.Persistence.Configurations;

public sealed class LeadConfiguration : IEntityTypeConfiguration<Lead>
{
    public void Configure(EntityTypeBuilder<Lead> builder)
    {
        builder.ToTable("leads");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasColumnName("id");

        builder.Property(x => x.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(x => x.Telefono)
            .HasColumnName("telefono")
            .HasMaxLength(50);

        builder.Property(x => x.ProductoInteres)
            .HasColumnName("producto_interes")
            .HasMaxLength(200);

        builder.Property(x => x.Presupuesto)
            .HasColumnName("presupuesto")
            .HasPrecision(18, 2);

        builder.Property(x => x.Fuente)
            .HasColumnName("fuente")
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(x => x.CreatedAtUtc)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(x => x.UpdatedAtUtc)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(x => x.DeletedAtUtc)
            .HasColumnName("deleted_at");

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("ix_leads_email");

        builder.HasQueryFilter(x => x.DeletedAtUtc == null);
    }
}
