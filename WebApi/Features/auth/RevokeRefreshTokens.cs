using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;

namespace WebApi.Features.auth;

public class RevokeRefreshTokens
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapDelete("api/auth/refresh-tokens", Handler)
                .WithTags("Auth")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handler(AppDbContext context,
        IHttpContextAccessor httpContextAccessor)
    {
        var currentUserId = Guid.TryParse(
            httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier),
            out var parsed)
            ? parsed
            : Guid.Empty;

        await context.RefreshTokens
            .Where(r => r.UserId == currentUserId)
            .ExecuteDeleteAsync();

        return Results.Ok(Result<string>.Ok("Success"));
    }
}