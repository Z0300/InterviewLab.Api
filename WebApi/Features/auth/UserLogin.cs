using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;
using WebApi.Utilities;

namespace WebApi.Features.auth;

public class UserLogin
{
    public record Request(string Username, string Password);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Username)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost("api/auth", Handler)
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

            return Results.BadRequest(Result<string>.ValidationError(errors));
        }

        var user = await context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(u => u.Username == req.Username, cancellationToken);

        if (user is null)
            return Results.BadRequest(Result<string>.Fail("Login failed. Please try again."));

        var verified = PasswordHasher.Verify(req.Password, user.PasswordHash);

        if (!verified)
            return Results.BadRequest(Result<string>.Fail("Login failed. Please try again."));

        var token = tokenProvider.Create(user);

        return Results.Ok(Result<string>.Ok(token));
    }
}