using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PRA.DevHabit.Data.Entities;

namespace PRA.DevHabit.Data.Configurations;

public sealed class HabitTagConfiguration : IEntityTypeConfiguration<HabitTag>
{
    public void Configure(EntityTypeBuilder<HabitTag> builder)
    {
        builder.HasKey(ht => new { ht.HabitId, ht.TagId });

        builder.HasOne<Tag>()
            .WithMany()
            .HasForeignKey(ht => ht.TagId);

        builder.HasOne<Habit>()
            .WithMany(h => h.HabitTags)
            .HasForeignKey(ht => ht.HabitId);
    }
}
