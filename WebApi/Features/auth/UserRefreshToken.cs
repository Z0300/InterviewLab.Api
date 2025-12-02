using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;
using WebApi.Utilities;

namespace WebApi.Features.auth;

public class UserRefreshToken
{
    public record Request(string RefreshToken);

    private record Response(string AccessToken, string RefreshToken);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty();
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost("api/auth/refresh-token", Handler)
                .WithTags("Auth");
        }
    }

    private static async Task<IResult> Handler(Request req,
        AppDbContext context,
        IValidator<Request> validator,
        TokenProvider tokenProvider,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(req, cancellationToken);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

            return Results.BadRequest(Result<Response>.ValidationError(errors));
        }

        var refreshToken = await context.RefreshTokens
            .Include(x => x.User)
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Token == req.RefreshToken, cancellationToken);

        if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
            return Results.BadRequest(Result<Response>.Fail("Refresh token not found"));

        var accessToken = tokenProvider.Create(refreshToken.User!);

        var (newRefreshToken, refreshExpiresAt) = tokenProvider.GenerateRefreshToken();

        refreshToken.Token = newRefreshToken;
        refreshToken.ExpiresAt = refreshExpiresAt;

        await context.SaveChangesAsync(cancellationToken);

        return Results.Ok(Result<Response>.Ok(new Response(
            accessToken,
            refreshToken.Token)));
    }
}