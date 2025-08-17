namespace BuildingBlocks.Results;

public class Result
{
  public bool IsSuccess { get; init; }

  public string? Error { get; init; }

  public string? Code { get; init; }

  public static Result Success()
  {
    return new Result { IsSuccess = true };
  }

  public static Result Failure(string error, string? code = null)
  {
    return new Result { IsSuccess = false, Error = error, Code = code };
  }
}

public class Result<T> : Result
{
	public T? Value { get; init; }

	public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
	public static new Result<T> Failure(string error, string? code = null) => new() { IsSuccess = false, Error = error, Code = code };
}