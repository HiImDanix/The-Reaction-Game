using FluentResults;

namespace Application.Errors;

public class InvalidInputError: Error
{
    private string Msg { get; }
    
    public InvalidInputError(string message): base(message)
    {
        Msg = message;
    }
}