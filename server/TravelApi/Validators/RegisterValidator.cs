
using FluentValidation;
using TravelApi.DTOs;

namespace TravelApi.Validators;

public class RegisterValidator : AbstractValidator<RegisterDto>
{
    public RegisterValidator()
    {
        // Проверка имени пользователя
        RuleFor(x => x.Username)
            .NotEmpty()
                .WithMessage("Введите имя пользователя")
            .MinimumLength(3)
                .WithMessage("Имя должно содержать минимум 3 символа")
            .MaximumLength(30)
                .WithMessage("Имя не должно превышать 30 символов")
            .Matches("^[a-zA-Z0-9_]+$")
                .WithMessage("Разрешены только латинские буквы, цифры и _");

        // Проверка электронной почты
        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("Введите электронную почту")
            .EmailAddress()
                .WithMessage("Некорректный email")
            .MaximumLength(254)
                .WithMessage("Email слишком длинный");

        // Проверка пароля
        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("Введите пароль")
            .MinimumLength(8)
                .WithMessage("Пароль должен содержать минимум 8 символов")
            .MaximumLength(128)
                .WithMessage("Пароль слишком длинный");
    }
}
