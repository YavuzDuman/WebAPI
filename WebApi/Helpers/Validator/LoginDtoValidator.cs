﻿using FluentValidation;
using WebApi.Entities.Concrete.Dtos;

namespace WebApi.Helpers.Validator
{
	public class LoginDtoValidator : AbstractValidator<LoginDto>
	{
		public LoginDtoValidator()
		{
			RuleFor(x => x.Username)
				.NotEmpty().WithMessage("Kullanıcı adı boş olamaz.");

			RuleFor(x => x.Password)
				.NotEmpty().WithMessage("Şifre boş olamaz.");
		}
	}
}
