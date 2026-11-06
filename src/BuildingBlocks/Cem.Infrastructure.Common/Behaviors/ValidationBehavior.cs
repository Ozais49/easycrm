using Cem.SharedKernel;
using FluentValidation;
using Mediator;

namespace Cem.Infrastructure.Common.Behaviors;

/// <summary>
/// Runs all FluentValidation validators for a message and short-circuits with a failed
/// <see cref="Result"/> (instead of throwing). Applies to messages whose response is a Result / Result&lt;T&gt;.
/// </summary>
public sealed class ValidationBehavior<TMessage, TResponse>(IEnumerable<IValidator<TMessage>> validators)
    : IPipelineBehavior<TMessage, TResponse>
    where TMessage : IMessage
    where TResponse : IFailable<TResponse>
{
    public async ValueTask<TResponse> Handle(
        TMessage message,
        MessageHandlerDelegate<TMessage, TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TMessage>(message);

        var failures = new List<FluentValidation.Results.ValidationFailure>();
        foreach (var validator in validators)
        {
            var result = await validator.ValidateAsync(context, cancellationToken);
            failures.AddRange(result.Errors);
        }

        if (failures.Count == 0)
        {
            return await next(message, cancellationToken);
        }

        var fieldErrors = failures
            .GroupBy(f => f.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(f => f.ErrorMessage).ToArray());

        return TResponse.Failure(Error.Validation("validation.failed", "One or more validation errors occurred.", fieldErrors));
    }
}
