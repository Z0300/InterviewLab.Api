using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;

namespace WebApi.Features.auth;

public class GetUser
{
    private record Response(Guid Id, string Username);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet("api/user" + "/{id:guid}", Handler)
                .WithTags("Auth");
        }
    }

    private static async Task<IResult> Handler(Guid id, AppDbContext context)
    {
        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (user is null)
            return Results.BadRequest(Result<Response>.Fail("The requested resource could not be processed."));

        var response = new Response(user.Id, user.Username);

        return Results.Ok(Result<Response>.Ok(
            response,
            "Successfully retrieved."
        ));
    }
}