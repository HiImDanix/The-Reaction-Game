using FluentResults;

namespace Application.Errors;

public class BusinessValidationError: Error
{
    private string Msg { get; }
    
    public BusinessValidationError(string message): base(message)
    {
        Msg = message;
    }
}