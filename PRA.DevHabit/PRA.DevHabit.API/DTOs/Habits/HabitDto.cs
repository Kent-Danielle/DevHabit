using PRA.DevHabit.Data.Entities;

namespace PRA.DevHabit.API.DTOs.Habits;

public sealed record HabitsCollectionDto
{
    public List<HabitDto> Data { get; init; }
}

// Naming convention choices
// 1. Request/Response
// CreateHabitRequest/Response

// 2. Model 
// CreateHabitModel/Model

// can do either classes or record

// NOTE: 
// adding required in the DTO properties allows us to have Required rule validation 
// without explicitly adding it throughout the application
// This is some basic validation implemented through having a DTO. There are other ways to apply more complex validation.
public sealed record HabitDto
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required HabitType Type { get; init; }
    public required FrequencyDto Frequency { get; init; }
    public required TargetDto Target { get; init; }
    public required HabitStatus Status { get; init; }
    public required bool IsArchived { get; init; }
    public DateOnly? EndDate { get; init; }
    public MileStoneDto? Milestone { get; init; }
    public required DateTime CreatedAtUtc { get; init; }
    public DateTime? UpdatedAtUtc { get; init; }
    public DateTime? LastCompletedAtUtc { get; init; }
}

public sealed record FrequencyDto
{
    public required FrequencyType Type { get; init; }
    public required int TimesPerPeriod { get; init; }
}

public sealed record TargetDto
{
    public required int Value { get; init; }
    public required string Unit { get; init; }
}

public sealed record MileStoneDto
{
    public required int Target { get; init; }
    public required int Current { get; init; }
}
