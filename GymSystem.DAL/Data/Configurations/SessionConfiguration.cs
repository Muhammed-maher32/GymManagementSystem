using GymSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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
