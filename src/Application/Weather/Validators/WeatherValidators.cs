using Application.Weather.Commands;
using Application.Weather.DTOs;
using FluentValidation;

namespace Application.Weather.Validators;

public class CreateWeatherRequestValidator : AbstractValidator<CreateWeatherRequest>
{
	public CreateWeatherRequestValidator()
	{
		RuleFor(x => x.LocationId).NotEmpty();
		RuleFor(x => x.Date).NotEmpty();
		RuleFor(x => x.TemperatureC).InclusiveBetween(-100, 100);
		RuleFor(x => x.Summary).MaximumLength(400);
	}
}

public class UpdateWeatherRequestValidator : AbstractValidator<UpdateWeatherRequest>
{
	public UpdateWeatherRequestValidator()
	{
		RuleFor(x => x.Date).NotEmpty();
		RuleFor(x => x.TemperatureC).InclusiveBetween(-100, 100);
		RuleFor(x => x.Summary).MaximumLength(400);
		RuleFor(x => x.RowVersion).NotNull().NotEmpty();
	}
}

public class CreateWeatherCommandValidator : AbstractValidator<CreateWeatherCommand>
{
	public CreateWeatherCommandValidator()
	{
		RuleFor(x => x.Request).SetValidator(new CreateWeatherRequestValidator());
	}
}

public class UpdateWeatherCommandValidator : AbstractValidator<UpdateWeatherCommand>
{
	public UpdateWeatherCommandValidator()
	{
		RuleFor(x => x.Id).NotEmpty();
		RuleFor(x => x.Request).SetValidator(new UpdateWeatherRequestValidator());
	}
}

public class DeleteWeatherCommandValidator : AbstractValidator<DeleteWeatherCommand>
{
	public DeleteWeatherCommandValidator()
	{
		RuleFor(x => x.Id).NotEmpty();
	}
}