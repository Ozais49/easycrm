using Cem.Settings.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.CustomFields;

/// <summary>Soft delete: values already stored in records stay there but are ignored.</summary>
public sealed class DeleteCustomFieldHandler(ICustomFieldStore store, TimeProvider timeProvider)
    : ICommandHandler<DeleteCustomFieldCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteCustomFieldCommand command, CancellationToken cancellationToken)
    {
        var definition = await store.FindAsync(command.Id, cancellationToken);
        if (definition is null)
        {
            return Result.Failure(CustomFieldErrors.NotFound);
        }

        definition.MarkDeleted(timeProvider.GetUtcNow());
        await store.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
