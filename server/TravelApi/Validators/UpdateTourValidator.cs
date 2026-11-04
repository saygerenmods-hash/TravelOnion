
using FluentValidation;
using TravelApi.DTOs;

namespace TravelApi.Validators;

public class UpdateTourValidator : AbstractValidator<UpdateTourDto>
{
    public UpdateTourValidator()
    {
        // Название тура обязательно
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Название тура обязательно")
            .MaximumLength(150)
            .WithMessage("Название не должно превышать 150 символов");

        // Цена должна быть больше нуля
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Цена должна быть больше нуля");

        // Количество мест должно быть положительным
        RuleFor(x => x.AvailablePlaces)
            .GreaterThan(0)
            .WithMessage("Количество мест должно быть больше нуля");

        // Страна должна быть указана
        RuleFor(x => x.CountryId)
            .GreaterThan(0)
            .WithMessage("Необходимо выбрать страну");

        // Дата начала обязательна
        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Укажите дату начала тура");

        // Дата окончания должна быть позже даты начала
        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("Дата окончания должна быть позже даты начала");

        // Тур не может длиться больше 30 дней
        RuleFor(x => x)
            .Must(x => (x.EndDate.Date - x.StartDate.Date).TotalDays <= 30)
            .When(x => x.StartDate != default &&
                       x.EndDate > x.StartDate)
            .WithName(nameof(UpdateTourDto.EndDate))
            .WithMessage("Продолжительность тура не может превышать 30 дней");
    }
}
