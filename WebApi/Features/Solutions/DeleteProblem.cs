using WebApi.Endpoints;

namespace WebApi.Features.Solutions;

public class DeleteProblem
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapDelete("api/solutions" + "/{id:guid}", Handler)
                .WithTags("Solutions");
        }
    }

    private static async Task<IResult> Handler(Guid id, AppDbContext context)
    {
        var problem = await context.Solutions.FindAsync(id);

        if (problem is null)
            return Results.BadRequest(Result<Guid>.Fail("The requested resource could not be processed."));

        context.Remove(problem);

        await context.SaveChangesAsync();

        return Results.Ok(Result<Guid>.Ok(id));
    }
}