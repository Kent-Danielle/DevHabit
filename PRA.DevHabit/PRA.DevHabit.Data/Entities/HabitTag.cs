using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRA.DevHabit.Data.Entities;

// This is the equivalent of a join table in pure SQL solution
public sealed class HabitTag
{
    public string HabitId { get; set; }
    public string TagId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
