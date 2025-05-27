using FluentValidation;
using StarWarsAPI.API.Models.Requests;
using System;

namespace StarWarsAPI.API.Validators
{
    public class CreateMovieRequestValidator : AbstractValidator<CreateMovieRequest>
    {
        public CreateMovieRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("El título es obligatorio.")
                .MaximumLength(200).WithMessage("El título no puede tener más de 200 caracteres.");

            RuleFor(x => x.OpeningCrawl)
                .NotEmpty().WithMessage("El texto inicial es obligatorio.");

            RuleFor(x => x.Director)
                .NotEmpty().WithMessage("El nombre del director es obligatorio.")
                .MaximumLength(100).WithMessage("El director no puede tener más de 100 caracteres.");

            RuleFor(x => x.Producer)
                .NotEmpty().WithMessage("El nombre del productor es obligatorio.")
                .MaximumLength(100).WithMessage("El productor no puede tener más de 100 caracteres.");

            RuleFor(x => x.ReleaseDate)
                .NotEmpty().WithMessage("La fecha de estreno es obligatoria.");
        }
    }
}
