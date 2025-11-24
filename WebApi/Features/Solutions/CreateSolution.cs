using FluentValidation;
using WebApi.Endpoints;
using WebApi.Models;

namespace WebApi.Features.Solutions;

public static class CreateSolution
{
    public record Request(
        Guid ProblemId,
        string Language,
        string Code,
        string? Explanation,
        bool IsCanonical,
        int QualityScore,
        string? Source);

    private record Response(Guid Id, Guid ProblemId);

    public sealed class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.ProblemId)
                .NotEmpty()
                .WithMessage("Property is not a valid GUID format.");

            RuleFor(x => x.Language)
                .NotEmpty()
                .MaximumLength(30);

            RuleFor(x => x.Code)
                .NotEmpty();

            RuleFor(x => x.Source)
                .MaximumLength(100);
        }
    }


    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost("api/solutions", Handler)
                .WithTags("Solutions");
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


        var solution = new Solution
        {
            Id = Guid.NewGuid(),
            ProblemId = req.ProblemId,
            Language = req.Language,
            Code = req.Code,
            Explanation = req.Explanation,
            IsCanonical = req.IsCanonical,
            QualityScore = req.QualityScore,
            Source = req.Source
        };

        context.Solutions.Add(solution);
        await context.SaveChangesAsync();

        return Results.Ok(Result<Response>.Ok(new Response(solution.Id, solution.ProblemId), "Successfully created"));
    }
}