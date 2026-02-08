using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PRA.DevHabit.API.DTOs.HabitTags;
using PRA.DevHabit.Data.Context;
using PRA.DevHabit.Data.Entities;

namespace PRA.DevHabit.API.Controllers;

//Note: Option 2 for handling child resource (i.e. join table)
//Have it as a separate controller class but still using the route from habits (like option 1 @ HabitController.cs)

[ApiController]
[Route("api/habits/{habitId}/tags")]
public sealed class HabitTagsController(ApplicationDbContext dbContext) : ControllerBase
{
    // Clarification: this upsert is for the join table, it does not upsert a new tag or a new habit
    // Design decision: we do upsert with tag collection so that client won't need multiple API calls to associate many tags under a habit
    [HttpPut]
    public async Task<ActionResult> UpsertHabitTags(string habitId, UpsertHabitTagsDto upsertHabitTagsDto)
    {
        Habit? habit = await dbContext.Habits
            .Include(h => h.HabitTags)
            .FirstOrDefaultAsync(h => h.Id == habitId);

        if (habit is null)
        {
            return NotFound();
        }

        var currentTagIds = habit.HabitTags.Select(ht => ht.TagId).ToHashSet();
        if (currentTagIds.SetEquals(upsertHabitTagsDto.TagIds)) // it means, there is nothing to update
        {
            return NoContent();
        }

        List<string> existingTagIds = await dbContext
            .Tags
            .Where(t => upsertHabitTagsDto.TagIds.Contains(t.Id))
            .Select(t => t.Id)
            .ToListAsync();

        if (existingTagIds.Count != upsertHabitTagsDto.TagIds.Count)
        {
            return BadRequest("One or more tag IDs is invalid");
        }

        // After all pre conditions, we can now implement upsert logic

        // Note: this is what makes it a true "PUT" endpoint as a true resource replacement
        // i.e. if the tag is not part of the request dto, we are also removing it under HabitTags
        habit.HabitTags.RemoveAll(ht => !upsertHabitTagsDto.TagIds.Contains(ht.TagId));

        string[] tagIdsToAdd = upsertHabitTagsDto.TagIds.Except(currentTagIds).ToArray();
        habit.HabitTags.AddRange(tagIdsToAdd.Select(tagId => new HabitTag
        {
            HabitId = habitId,
            TagId = tagId,
            CreatedAtUtc = DateTime.UtcNow,
        }));

        await dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{tagId}")]
    public async Task<ActionResult> DeleteHabitTag(string habitId, string tagId)
    {
        HabitTag? habitTag = await dbContext.HabitTags
            .SingleOrDefaultAsync(ht => ht.HabitId == habitId && ht.TagId == tagId);

        if (habitTag is null)
        {
            return NotFound();
        }

        dbContext.HabitTags.Remove(habitTag);

        await dbContext.SaveChangesAsync();

        return NoContent();
    }
}
