using GymSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(p => p.Name).HasColumnType("NVarchar").HasMaxLength(50);

            builder.Property(p => p.Description).HasMaxLength(200);

            builder.Property(p => p.Price).HasPrecision(10, 2);

            builder.Property(p => p.CreatedAt).HasDefaultValueSql("GetDate()");

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("DurationsDaysContraint", "DurationDays Between 1 and 365");
            });
        }
    }
}
