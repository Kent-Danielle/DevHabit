using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRA.DevHabit.Data.Entities;

public sealed class Habit
{
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public HabitType Type { get; set; }
    public Frequency Frequency { get; set; }
    public Target Target { get; set; }
    public HabitStatus Status { get; set; }
    public bool IsArchived { get; set; } // Separate from status, it's for data archiving
    public DateOnly? EndDate { get; set; }
    public Milestone? Milestone { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; } // Optional, not all data will be updated
    public DateTime? LastCompletedAtUtc { get; set; }
}

public enum HabitType
{
    None = 0,
    Binary = 1, // complete or not
    Numeric = 2, // measurable
}

public enum HabitStatus
{
    None = 0,
    Ongoing = 1,
    Completed = 2,
}

public sealed class Frequency
{
    public FrequencyType Type { get; set; }
    public int TimesPerPeriod { get; set; }
}

public enum FrequencyType
{
    None = 0,
    Daily = 1,
    Weekly = 2,
    Monthly = 3,
}

public sealed class Target
{
    public int Value { get; set; }
    public string Unit { get; set; }
}

public sealed class Milestone
{
    public int Target { get; set; }
    public int Current { get; set; }
}
