using GymSystem.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.Id);

            builder.HasKey(x => new {x.SessionId, x.MemberId});
            builder.Property(x => x.BookingDate).HasDefaultValueSql("GetDate()");
        }
    }
}
