using System.Text.Json;
using FluentValidation;
using WebApi.Endpoints;
using WebApi.Models;
using WebApi.Models.Enums;

namespace WebApi.Features.Problems;

public class CreateProblem
{
    public record Request(
        string Title,
        string Company,
        string Description,
        Difficulty Difficulty,
        string[]? TagsJson);

    private record Response(Guid Id, string Title);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .NotEmpty();

            RuleFor(x => x.Difficulty)
                .IsInEnum();
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost("api/problems", Handler)
                .WithTags("Problems")
                .RequireAuthorization();
        }
    }

    private static async Task<IResult> Handler(Request req, AppDbContext context, IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(req);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

            return Results.BadRequest(Result<Response>.ValidationError(errors));
        }

        var problem = new Problem
        {
            Id = Guid.NewGuid(),
            Company = req.Company,
            Title = req.Title,
            Description = req.Description,
            Difficulty = Enum.Parse<Difficulty>(req.Difficulty.ToString()),
            TagsJson = req.TagsJson
        };

        context.Problems.Add(problem);
        await context.SaveChangesAsync();

        return Results.Ok(Result<Response>.Ok(new Response(problem.Id, problem.Title), "Successfully created"));
    }
}