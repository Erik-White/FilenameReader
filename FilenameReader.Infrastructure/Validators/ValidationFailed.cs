using FluentValidation.Results;

namespace FilenameReader.Infrastructure.Validators;

public record ValidationFailed(IEnumerable<ValidationFailure> Errors)
{
    public ValidationFailed(ValidationFailure error) : this(new[] { error })
    {
    }

    public IEnumerable<string> ErrorMessages
        => Errors.Select(error => error.ErrorMessage);
}
