namespace Cem.SharedKernel;

public class Result : IFailable<Result>
{
    private readonly Error? _error;

    protected Result(Error? error) => _error = error;

    public bool IsSuccess => _error is null;
    public bool IsFailure => !IsSuccess;

    public Error Error => _error ?? throw new InvalidOperationException("A successful result has no error.");

    public static Result Success() => new(null);

    public static Result Failure(Error error) => new(error);
}
