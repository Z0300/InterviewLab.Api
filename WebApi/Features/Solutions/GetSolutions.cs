using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;

namespace WebApi.Features.Solutions;

public class GetSolutions
{
    private record Response(
        Guid Id,
        string Language,
        string Code,
        string? Explanation,
        bool IsCanonical,
        int QualityScore,
        string? Source);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet("api/solutions", Handler)
                .WithTags("Solutions");
        }
    }

    private static async Task<IResult> Handler(AppDbContext context)
    {
        var solutions = await context.Solutions
            .Include(x => x.Problem)
            .AsNoTracking()
            .Select(e => new Response(
                e.Id,
                e.Language,
                e.Code,
                e.Explanation,
                e.IsCanonical,
                e.QualityScore,
                e.Source))
            .ToListAsync();

        return Results.Ok(Result<List<Response>>.Ok(
            solutions,
            "Successfully retrieved."
        ));
    }
}