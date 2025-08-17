namespace BuildingBlocks.Pagination;

public class PagedResult<T>
{
	public required IReadOnlyCollection<T> Items { get; init; }
	public int Page { get; init; }
	public int PageSize { get; init; }
	public long TotalItems { get; init; }
	public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}