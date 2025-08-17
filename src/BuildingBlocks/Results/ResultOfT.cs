namespace BuildingBlocks.Results;

public class Result<T> : Result
{
  public T? Value { get; init; }

  public static Result<T> Success(T value)
  {
    return new Result<T> { IsSuccess = true, Value = value };
  }

  public static Result<T> Failure(string error, string? code = null)
  {
    return new Result<T> { IsSuccess = false, Error = error, Code = code };
  }
}