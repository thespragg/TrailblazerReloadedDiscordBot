namespace Domain.Models;

public record Unit;
public class Result<T>
{
    public Error? Error { get; set; }
    public T? Value { get; set; }
    public bool IsSuccess => Value is not null;

    public static Result<T> Ok(T result)
        => new()
        {
            Value = result
        };
    
    public static Result<T> Failed(string err)
        => new()
        {
            Error = new Error(err)
        };
}