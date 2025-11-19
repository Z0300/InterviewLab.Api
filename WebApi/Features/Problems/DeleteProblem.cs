using WebApi.Endpoints;

namespace WebApi.Features.Problems;

public class DeleteProblem
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/problems" + "/{id:guid}", Handler)
                .WithTags("Problems");
        }
    }

    private static async Task<IResult> Handler(Guid id, AppDbContext context)
    {
        var problem = await context.Problems.FindAsync(id);

        if (problem is null)
            return Results.BadRequest(Result<Guid>.Fail("The requested resource could not be processed."));


        context.Remove(problem);

        await context.SaveChangesAsync();

        return Results.Ok(Result<Guid>.Ok(id));
    }
}