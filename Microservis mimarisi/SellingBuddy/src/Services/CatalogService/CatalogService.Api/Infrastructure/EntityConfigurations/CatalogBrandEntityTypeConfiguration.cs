﻿using Microsoft.EntityFrameworkCore;
using CatalogService.Api.Infrastructure.Context;
using CatalogService.Api.Core.Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityConfigurations
{
    public class CatalogBrandEntityTypeConfiguration:IEntityTypeConfiguration<CatalogBrand>

    {
       
public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
           builder.ToTable("CatalogBrand",CatalogContext.DEFAULT_SCHEMA);
            builder.HasKey(ci=>ci.Id);
            builder.Property(ci => ci.Id)
                .UseHiLo("catalog_brand_hilo")
                .IsRequired();

            builder.Property(cb => cb.Brand)
                .IsRequired()
                .HasMaxLength(100);

        }
    }

}
