namespace CommandParser.Contracts;

public interface IParserResult
{
    Task<bool> InvokeAsync();
    Action<string>? OnError { get; set; }
}

public interface IParserResult<in TContext>
{
    Task<bool> InvokeAsync();
    Task<bool> InvokeAsync(TContext? ctx);
    Action<string>? OnError { get; set; }
}