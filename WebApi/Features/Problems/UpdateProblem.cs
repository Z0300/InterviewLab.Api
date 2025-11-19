using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;
using WebApi.Models;

namespace WebApi.Features.Problems;

public class UpdateProblem
{
    public record Request(string Title, string Description, string Difficulty, string TagsJson);


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
            builder.MapPut("api/problems" + "/{id:guid}", Handler)
                .WithTags("Problems");
        }
    }

    private static async Task<IResult> Handler(Guid id, Request req, AppDbContext context,
        IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(req);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

            return Results.BadRequest(Result<Guid>.ValidationError(errors));
        }

        var problem = await context.Problems.FirstOrDefaultAsync(x => x.Id == id);

        if (problem is null)
            return Results.BadRequest(Result<Guid>.Fail("The requested resource could not be processed."));

        problem.Title = req.Title;
        problem.Description = req.Description;
        problem.Difficulty = req.Difficulty;
        problem.TagsJson = req.TagsJson;

        await context.SaveChangesAsync();

        return Results.Ok(Result<Guid>.Ok(id, "Successfully updated"));
    }
}