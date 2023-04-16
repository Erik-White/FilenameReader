using FilenameReader.Core;
using FluentValidation;

namespace FilenameReader.Infrastructure.Validators
{
    public class FilePathValidator : AbstractValidator<FilePath>
    {
        public FilePathValidator()
        {
            RuleFor(x => x.FullPath)
                .NotNull()
                .NotEmpty()
                .WithMessage("A file path must be provided");

            RuleFor(x => x.Filename)
                .NotNull()
                .NotEmpty()
                .WithMessage("No suitable filename could be found");
        }
    }
}
