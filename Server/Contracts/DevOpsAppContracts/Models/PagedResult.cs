using System.ComponentModel.DataAnnotations;

namespace DevOpsAppContracts.Models;

public class PagedResult<T>
{
    [Required]
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    [Required]
    public int TotalCount { get; init; }
    [Required]
    public int Page { get; init; }
    [Required]
    public int PageSize { get; init; }
}