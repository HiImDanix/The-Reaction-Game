using Contracts.Input;
using FluentValidation;

namespace ReaktlyC.Validators;

public class JoinRoomValidator: AbstractValidator<JoinRoomReq>
{
    public JoinRoomValidator()
    {
        RuleFor(x => x.PlayerName).Cascade(CascadeMode.Stop).NotEmpty().Length(3, 20);
    }
}