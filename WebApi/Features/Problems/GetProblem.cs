using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;

namespace WebApi.Features.Problems;

public class GetProblem
{
    public record Response(
        Guid Id,
        string Title,
        string Description,
        string Difficulty,
        string? TagsJson);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet("api/problems" + "/{id:guid}", Handler)
                .WithTags("Problems");
        }
    }

    private static async Task<IResult> Handler(Guid id, AppDbContext context)
    {
        var problem = await context.Problems
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (problem is null)
            return Results.BadRequest(Result<Response>.Fail("The requested resource could not be processed."));

        var response = new Response(
            problem.Id,
            problem.Title,
            problem.Description,
            problem.Difficulty,
            problem.TagsJson);

        return Results.Ok(Result<Response>.Ok(
            response,
            "Successfully retrieved."
        ));
    }
}