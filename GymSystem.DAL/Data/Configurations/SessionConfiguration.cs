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
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.Property(x => x.CreatedAt).HasColumnName("StartDate").HasDefaultValueSql("GetDate()");


            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("CapacityConstraint", "Capacity Between 1 and 25");
                tb.HasCheckConstraint("TimeConstraint", "StartDate < EndDate");
            });

        }
    }
}
