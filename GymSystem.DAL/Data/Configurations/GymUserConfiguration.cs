using GymSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Data.Configurations
{
    public class GymUserConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : GymUser, new()
    {
        public void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GetDate()");

            builder.Property(x => x.Name).HasMaxLength(50);

            builder.Property(x => x.Email).HasMaxLength(100);

            builder.HasIndex(x => x.Email).IsUnique();
            builder.HasIndex(x => x.Phone).IsUnique();


            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("EmailConstraint", "Email Like '_%@_%._%'");
                tb.HasCheckConstraint("PhoneConstraint", "Phone Like '010%' or Phone Like '011%'");
            });


            builder.OwnsOne(x => x.Address, address =>
            {
                address.Property(a => a.City).HasColumnName("City");
                address.Property(a => a.Street).HasColumnName("Street");
                address.Property(a => a.BuildingNumber).HasColumnName("BuildingNumber");
            });


        }
    }
}
