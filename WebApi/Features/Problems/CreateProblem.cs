using FluentValidation;
using WebApi.Endpoints;
using WebApi.Models;

namespace WebApi.Features.Problems;

public class CreateProblem
{
    public record Request(
        string Title,
        string Description,
        string Difficulty,
        string TagsJson);

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
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.TagsJson)
                .MaximumLength(300);
        }
    }

    public sealed class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost("api/problems", Handler)
                .WithTags("Problems");
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
            Title = req.Title,
            Description = req.Description,
            Difficulty = req.Difficulty,
            TagsJson = req.TagsJson
        };

        context.Problems.Add(problem);
        await context.SaveChangesAsync();

        return Results.Ok(Result<Response>.Ok(new Response(problem.Id, problem.Title), "Successfully created"));
    }
}