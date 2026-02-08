using System.Linq.Expressions;
using PRA.DevHabit.Data.Entities;

namespace PRA.DevHabit.API.DTOs.Tags;

internal static class TagQueries
{
    public static Expression<Func<Tag, TagDto>> ProjectToDto()
    {
        return t => new TagDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            CreatedAtUtc = t.CreatedAtUtc,
            UpdatedAtUtc = t.UpdatedAtUtc
        };
    }
}
