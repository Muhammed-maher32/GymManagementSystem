using GymSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymSystem.DAL.Data.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.Id);

            builder.HasKey(x => new { x.SessionId, x.MemberId });
            builder.Property(x => x.BookingDate).HasDefaultValueSql("GetDate()");
        }
    }
}
