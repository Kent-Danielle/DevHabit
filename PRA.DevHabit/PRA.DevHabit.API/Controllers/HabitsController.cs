using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRA.DevHabit.API.DTOs.Habits;
using PRA.DevHabit.Data.Context;
using PRA.DevHabit.Data.Entities;

namespace PRA.DevHabit.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HabitsController(ApplicationDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<HabitsCollectionDto>> GetHabits()
    {
        List<HabitDto> habits = await dbContext.Habits
            .Select(h => new HabitDto
            {
                Id = h.Id.ToString(),
                Name = h.Name,
                Description = h.Description,
                Type = h.Type,
                Frequency = new FrequencyDto
                {
                    Type = h.Frequency.Type,
                    TimesPerPeriod = h.Frequency.TimesPerPeriod
                },
                Target = new TargetDto
                {
                    Value = h.Target.Value,
                    Unit = h.Target.Unit
                },
                Status = h.Status,
                IsArchived = h.IsArchived,
                EndDate = h.EndDate,
                Milestone = h.Milestone == null ? null : new MileStoneDto
                {
                    Target = h.Milestone.Target,
                    Current = h.Milestone.Current
                },
                CreatedAtUtc = h.CreatedAtUtc,
                UpdatedAtUtc = h.UpdatedAtUtc,
                LastCompletedAtUtc = h.LastCompletedAtUtc
            })
            .ToListAsync();

        var habitsCollectionDto = new HabitsCollectionDto
        {
            Data = habits
        };

        return Ok(habitsCollectionDto);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<HabitDto>> GetHabit(string id)
    {
        HabitDto? habit = await dbContext
            .Habits
            .Where(h => h.Id == id)
            .Select(h => new HabitDto
            {
                Id = h.Id.ToString(),
                Name = h.Name,
                Description = h.Description,
                Type = h.Type,
                Frequency = new FrequencyDto
                {
                    Type = h.Frequency.Type,
                    TimesPerPeriod = h.Frequency.TimesPerPeriod
                },
                Target = new TargetDto
                {
                    Value = h.Target.Value,
                    Unit = h.Target.Unit
                },
                Status = h.Status,
                IsArchived = h.IsArchived,
                EndDate = h.EndDate,
                Milestone = h.Milestone == null ? null : new MileStoneDto
                {
                    Target = h.Milestone.Target,
                    Current = h.Milestone.Current
                },
                CreatedAtUtc = h.CreatedAtUtc,
                UpdatedAtUtc = h.UpdatedAtUtc,
                LastCompletedAtUtc = h.LastCompletedAtUtc
            })
            .FirstOrDefaultAsync();

        return habit is null ? NotFound() : Ok(habit);
    }
}
