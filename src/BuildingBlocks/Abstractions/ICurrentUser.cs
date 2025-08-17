namespace BuildingBlocks.Abstractions;

public interface ICurrentUser
{
  string? UserId { get; }

  string? UserName { get; }

  bool IsAuthenticated { get; }

  IReadOnlyCollection<string> Roles { get; }
}