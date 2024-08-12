namespace BackendChallenge.CrossCutting.Common;
public class Error
{
    public Error(string code, string message)
    {
        Code = code;
        Message = message;
    }

    public Error(string code, string[] messages)
    {
        Code = code;
        Message = string.Join("; ", messages);
    }

    public string Code { get; }
    public string Message { get; }

    public static Error None = new(string.Empty, string.Empty);
    public static Error NullValue = new("Error.NullValue", "Null value was provided");
}
