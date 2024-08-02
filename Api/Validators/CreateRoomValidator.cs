using Contracts.Input;
using FluentValidation;

namespace ReaktlyC.Validators;

public class CreateRoomValidator: AbstractValidator<CreateRoomReq>
{
    public CreateRoomValidator()
    {
        RuleFor(x => x.PlayerName).Cascade(CascadeMode.Stop).NotEmpty().Length(3, 20);
    }
}