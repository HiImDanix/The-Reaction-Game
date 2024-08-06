using FluentResults;

namespace Application.Errors;

public class NotFoundError: Error
{
    private string Msg { get; }
    
    public NotFoundError(string message): base(message)
    {
        Msg = message;
    }
}