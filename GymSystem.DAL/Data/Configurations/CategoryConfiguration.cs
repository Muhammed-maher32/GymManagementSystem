using GymSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(x => x.Name).HasMaxLength(50);
            builder.Property(x => x.CreatedAt).HasDefaultValueSql("GetDate()");
            builder.HasData(
                new Category { Id = 1, Name = "Cardio" },
                new Category { Id = 2, Name = "Strength" },
                new Category { Id = 3, Name = "Yoga" },
                new Category { Id = 4, Name = "Boxing" },
                new Category { Id = 5, Name = "CrossFit" }
            );


        }
    }
}
