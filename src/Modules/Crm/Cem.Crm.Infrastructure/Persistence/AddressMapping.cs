using Cem.Crm.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cem.Crm.Infrastructure.Persistence;

internal static class AddressMapping
{
    /// <summary>Stores the address as address_* columns of the owning table.</summary>
    public static void Configure(ComplexPropertyBuilder<Address> address)
    {
        address.Property(a => a.Street).HasMaxLength(200);
        address.Property(a => a.City).HasMaxLength(100);
        address.Property(a => a.District).HasMaxLength(100);
        address.Property(a => a.Province).HasConversion<string>().HasMaxLength(20);
    }
}
