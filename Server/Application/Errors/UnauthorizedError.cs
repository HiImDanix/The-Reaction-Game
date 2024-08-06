using FluentResults;

namespace Application.Errors;

public class UnauthorizedError: Error
{
    private string Msg { get; }
    
    public UnauthorizedError(string message): base(message)
    {
        Msg = message;
    }
}