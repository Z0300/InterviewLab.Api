using Microsoft.EntityFrameworkCore;
using WebApi.Endpoints;
using WebApi.Models.Enums;

namespace WebApi.Features.Problems;

public class GetProblems
{
    private record PagedResult<T>(
        IReadOnlyList<T> Items,
        int Page,
        int PageSize,
        int TotalCount,
        int TotalPages);


    private record Response(
        Guid Id,
        string Title,
        string Company,
        string Description,
        Difficulty Difficulty,
        string[]? TagsJson);

    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet("api/problems", Handler)
                .WithTags("Problems");
        }
    }

    private static async Task<IResult> Handler(AppDbContext context,
        int page = 1, int pageSize = 10, string? search = null)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        const int maxPageSize = 100;
        if (pageSize > maxPageSize) pageSize = maxPageSize;

        var query = context.Problems
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalized = search.Trim();
            query = query.Where(e =>
                EF.Functions.Like(e.Title, $"%{normalized}%")
            );
        }

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .OrderBy(e => e.Title) // deterministic ordering; change as needed
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(e => new Response(
                e.Id,
                e.Title,
                e.Company,
                e.Description,
                e.Difficulty,
                e.TagsJson
            ))
            .ToListAsync();

        var paged = new PagedResult<Response>(
            Items: items,
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount,
            TotalPages: totalPages
        );

        return Results.Ok(Result<PagedResult<Response>>.Ok(
            paged,
            "Successfully retrieved."
        ));
    }
}