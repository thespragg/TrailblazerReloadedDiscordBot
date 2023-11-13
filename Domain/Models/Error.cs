namespace Domain.Models;

public class Error
{
    public Error(string msg)
    {
        Message = msg;
    }

    public string Message { get; set; }
}