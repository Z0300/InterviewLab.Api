using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;
using WebApi.Models.Enums;

namespace WebApi.Features.Solutions;

public class GetSolution
{
    private record Response(
        Guid Id,
        ProblemResponse Problem,
        string Language,
        string Code,
        string? Explanation,
        bool IsCanonical,
        int QualityScore,
        string? Source);

    private record ProblemResponse(
        Guid Id,
        string Title,
        string Description,
        Difficulty Difficulty,
        string[]? TagsJson);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet("api/solutions" + "/{id:guid}", Handler)
                .WithTags("Solutions");
        }
    }

    private static async Task<IResult> Handler(Guid id, AppDbContext context)
    {
        var solution = await context.Solutions
            .Include(x => x.Problem)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (solution is null)
            return Results.BadRequest(Result<Response>.Fail("The requested resource could not be processed."));

        var response = new Response(
            solution.Id,
            new ProblemResponse(
                solution.Problem!.Id,
                solution.Problem.Title,
                solution.Problem.Description,
                solution.Problem.Difficulty,
                solution.Problem.TagsJson),
            solution.Language,
            solution.Code,
            solution.Explanation,
            solution.IsCanonical,
            solution.QualityScore,
            solution.Source);

        return Results.Ok(Result<Response>.Ok(
            response,
            "Successfully retrieved."
        ));
    }
}