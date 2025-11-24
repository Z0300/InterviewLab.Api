using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;
using WebApi.Models.Enums;

namespace WebApi.Features.Problems;

public class GetProblems
{
    private record Response(
        Guid Id,
        string Title,
        string Company,
        string Description,
        Difficulty Difficulty,
        string[]? TagsJson);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet("api/problems", Handler)
                .WithTags("Problems");
        }
    }

    private static async Task<IResult> Handler(AppDbContext context)
    {
        var problems = await context.Problems
            .AsNoTracking()
            .Select(e => new Response(
                e.Id,
                e.Title,
                e.Company,
                e.Description,
                e.Difficulty,
                e.TagsJson
            ))
            .ToListAsync();

        return Results.Ok(Result<List<Response>>.Ok(
            problems,
            "Successfully retrieved."
        ));
    }
}