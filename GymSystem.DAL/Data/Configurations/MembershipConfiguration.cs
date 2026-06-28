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
    public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
    {
        public void Configure(EntityTypeBuilder<Membership> builder)
        {

            builder.Ignore(x => x.CreatedAt);
            builder.Ignore(x => x.Id);

            builder.HasKey(x => new { x.PlanId, x.MemberId });
            builder.Property(x => x.StartDate).HasDefaultValueSql("GetDate()");
        }
    }
}
