using FluentValidation;
using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;

namespace WebApi.Features.Solutions;

public class UpdateSolution
{
    public record Request(
        Guid ProblemId,
        string Language,
        string Code,
        string? Explanation,
        bool IsCanonical,
        int QualityScore,
        string? Source);

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
            builder.MapPut("api/solutions" + "/{id:guid}", Handler)
                .WithTags("Solutions");
        }
    }

    private static async Task<IResult> Handler(Guid id, Request req, AppDbContext context,
        IValidator<Request> validator)
    {
        var validationResult = await validator.ValidateAsync(req);

        if (!validationResult.IsValid)
            return Results.BadRequest(Result<Guid>.Fail("Validation failed"));

        var solution = await context.Solutions.FirstOrDefaultAsync(x => x.Id == id);

        if (solution is null)
            return Results.BadRequest(Result<Guid>.Fail("The requested resource could not be processed."));

        solution.Language = req.Language;
        solution.Code = req.Code;
        solution.Explanation = req.Explanation;
        solution.IsCanonical = req.IsCanonical;
        solution.QualityScore = req.QualityScore;
        solution.Source = req.Source;

        await context.SaveChangesAsync();

        return Results.Ok(Result<Guid>.Ok(id, "Successfully updated"));
    }
}