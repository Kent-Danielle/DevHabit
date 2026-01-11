using PRA.DevHabit.Data.Entities;

namespace PRA.DevHabit.API.DTOs.Habits;

// Consideration: create a base class for shared props between CreateHabitDto and HabitDto
// Counter-point: it creates some sort of coupling
// Pragmatic: code duplication is ok for this
public sealed record CreateHabitDto
{
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required HabitType Type { get; init; }
    public required FrequencyDto Frequency { get; init; }
    public required TargetDto Target { get; init; }
    public DateOnly? EndDate { get; init; }
    public MileStoneDto? Milestone { get; init; }
 }
