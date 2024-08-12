namespace BackendChallenge.CrossCutting.Common;
public class Result
{
    protected internal Result(bool isSuccess, Error error)
    {
        if (isSuccess && error != Common.Error.None)
            throw new InvalidOperationException();

        if (!isSuccess && error == Common.Error.None)
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        if (error != Common.Error.None) Error.Add(error);
    }

    protected internal Result(bool isSuccess, List<Error> error)
    {
        if (isSuccess && error.Any())
            throw new InvalidOperationException();

        if (!isSuccess && !error.Any())
            throw new InvalidOperationException();

        IsSuccess = isSuccess;
        Error.AddRange(error);
    }

    protected internal Result(int count, bool isSuccess, Error error)
    {
        if (isSuccess && error != Common.Error.None)
            throw new InvalidOperationException();

        if (!isSuccess && error == Common.Error.None)
            throw new InvalidOperationException();

        Count = count;
        IsSuccess = isSuccess;
        if (error != Common.Error.None) Error.Add(error);
    }

    protected internal Result(int count, bool isSuccess, List<Error> error)
    {
        if (isSuccess && error.Any())
            throw new InvalidOperationException();

        if (!isSuccess && !error.Any())
            throw new InvalidOperationException();

        Count = count;
        IsSuccess = isSuccess;
        Error.AddRange(error);
    }

    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public int Count { get; }
    public List<Error> Error { get; } = new List<Error>();

    public static Result Success() => new(true, Common.Error.None);
    public static Result Failure(Error error) => new(false, error);
    public static Result Failure(List<Error> error) => new(false, error);
    public static Result<TValue> Success<TValue>(TValue value) => new(value, true, Common.Error.None);
    public static Result<TValue> Success<TValue>(int count, TValue value) => new(count, value, true, Common.Error.None);
    public static Result<TValue> Failure<TValue>(Error error) => new(default!, false, error);
    public static Result<TValue> Failure<TValue>(List<Error> error) => new(default!, false, error);
    public static Result Failure(string code, IEnumerable<string> errorDescriptions)
    {
        if (errorDescriptions == null || !errorDescriptions.Any())
            throw new ArgumentException("Error descriptions cannot be empty or null.");

        return new(false, new Error(code, errorDescriptions.ToArray()));
    }

    public static Result<TValue> Create<TValue>(TValue value) =>
        value != null ? Success(value) : Failure<TValue>(Common.Error.NullValue);
}

public class Result<TValue> : Result
{
    private readonly TValue _value;

    protected internal Result(TValue value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    protected internal Result(TValue value, bool isSuccess, List<Error> error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    protected internal Result(int count, TValue value, bool isSuccess, Error error)
        : base(count, isSuccess, error)
    {
        _value = value;
    }

    protected internal Result(int count, TValue value, bool isSuccess, List<Error> error)
        : base(count, isSuccess, error)
    {
        _value = value;
    }

    public TValue Value => _value ?? default!;

    public static implicit operator Result<TValue>(TValue value) => Create(value);
}