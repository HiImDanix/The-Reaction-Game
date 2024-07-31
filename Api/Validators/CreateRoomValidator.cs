using Contracts.Input;
using FluentValidation;

namespace ReaktlyC.Validators;

public class CreateRoomValidator: AbstractValidator<CreateRoomRequest>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.PlayerName).Length(3, 20);
    }
}