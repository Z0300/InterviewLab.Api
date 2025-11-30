using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;

namespace WebApi.Features.auth;

public class GetCurrentUser
{
    private record Response(Guid Id, string Username);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet("api/auth/me", Handler)
                .WithTags("Auth")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handler(HttpContext http, AppDbContext context)
    {
        var userPrincipal = http.User;
        if (userPrincipal.Identity is null || !userPrincipal.Identity.IsAuthenticated)
            return Results.Unauthorized();

        var userIdClaim = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? userPrincipal.FindFirst("sub")?.Value
                          ?? userPrincipal.FindFirst("id")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return Results.Unauthorized();

        var user = await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == userId);

        if (user is null)
            return Results.Unauthorized();

        var response = new Response(user.Id, user.Username);

        return Results.Ok(Result<Response>.Ok(
            response,
            "Successfully retrieved."
        ));
    }
}