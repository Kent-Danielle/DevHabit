using System.Linq.Expressions;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
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
            .Select(HabitQueries.ProjectToDto())
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
            .Select(HabitQueries.ProjectToDto())
            .FirstOrDefaultAsync();

        return habit is null ? NotFound() : Ok(habit);
    }

    [HttpPost]
    public async Task<ActionResult<HabitDto>> CreateHabit(CreateHabitDto createHabitDto)
    {
        Habit habit = createHabitDto.ToEntity();

        dbContext.Habits.Add(habit);

        await dbContext.SaveChangesAsync();

        HabitDto habitDto = habit.ToDto();

        // Using CreatedAtAction returns a Location in header response that can be called as an endpoint
        return CreatedAtAction(nameof(GetHabit), new { id = habitDto.Id }, habitDto); // Response code is 201 created, most appropriate for create requests
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateHabit(string id, UpdateHabitDto updateHabitDto)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null)
        {
            return NotFound();
        }

        habit.UpdateFromDto(updateHabitDto);

        await dbContext.SaveChangesAsync();

        return NoContent(); // Common PUT http response
    }

    /* NOTES:
     * It kind of mirrors a useReducer pattern in React
     * In reality, patch is not practical because:
     * 1. Client has to know how to construct a JsonPatchDocument
     * 2. It would be hard to add business logic into it, and at that point, why don't you just make it a PUT endpoint?
     * 
     * Options:
     * 1. Use PUT for partial updates instead, this makes it so that you send out a representation of the resource rather than a JsonPatch Document
     * 2. Create individual PATCH endpoints per field we want to update (i.e. PATCH endpoint for archiving a habit, or updating the frequency, etc)
     */
    [HttpPatch("{id}")]
    public async Task<ActionResult> PatchHabit(string id, JsonPatchDocument<HabitDto> patchDocument)
    {
        Habit? habit = await dbContext.Habits.FirstOrDefaultAsync(h => h.Id == id);

        if (habit is null)
        {
            return NotFound();
        }

        HabitDto habitDto = habit.ToDto();

        patchDocument.ApplyTo(habitDto, ModelState);

        // this is more robust than ModelState.IsValid - this lets required fields get removed, while TryValidateModel does not
        if (!TryValidateModel(habitDto))
        {
            return ValidationProblem(ModelState);
        }

        habit.Name = habitDto.Name;
        habit.Description = habitDto.Description;
        habit.UpdatedAtUtc = habitDto.UpdatedAtUtc;

        await dbContext.SaveChangesAsync(); 

        return NoContent();
    }
}
