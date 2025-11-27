using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;
using WebApi.Models.Enums;

namespace WebApi.Features.Problems;

public class GetProblem
{
    private record SolutionResponse(
        Guid Id,
        string Language,
        string Code,
        string? Explanation,
        bool IsCanonical,
        int QualityScore,
        string? Source);

    private record Response(
        Guid Id,
        string Title,
        string Company,
        string Description,
        Difficulty Difficulty,
        string[]? TagsJson,
        List<SolutionResponse>? Solutions);

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
            .Include(c => c.Solutions)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (problem is null)
            return Results.BadRequest(Result<Response>.Fail("The requested resource could not be processed."));

        var response = new Response(
            problem.Id,
            problem.Title,
            problem.Company,
            problem.Description,
            problem.Difficulty,
            problem.TagsJson,
            problem.Solutions
                .Select(solution => new SolutionResponse(
                    solution.Id,
                    solution.Language,
                    solution.Code,
                    solution.Explanation,
                    solution.IsCanonical,
                    solution.QualityScore,
                    solution.Source
                ))
                .ToList()
        );

        return Results.Ok(Result<Response>.Ok(
            response,
            "Successfully retrieved."
        ));
    }
}