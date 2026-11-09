using Cem.Settings.Application.Abstractions;
using Cem.Settings.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.CustomFields;

public sealed class UpdateCustomFieldHandler(ICustomFieldStore store)
    : ICommandHandler<UpdateCustomFieldCommand, Result<CustomFieldDto>>
{
    public async ValueTask<Result<CustomFieldDto>> Handle(UpdateCustomFieldCommand command, CancellationToken cancellationToken)
    {
        var definition = await store.FindAsync(command.Id, cancellationToken);
        if (definition is null)
        {
            return Result<CustomFieldDto>.Failure(CustomFieldErrors.NotFound);
        }

        string[] options;
        if (definition.Type == CustomFieldType.Select)
        {
            if (command.Options is not { Count: >= 1 and <= 50 })
            {
                return Result<CustomFieldDto>.Failure(CustomFieldErrors.Invalid("Options", "A select field needs 1 to 50 options."));
            }

            options = command.Options.Select(o => o.Trim()).ToArray();
        }
        else if (command.Options is { Count: > 0 })
        {
            return Result<CustomFieldDto>.Failure(CustomFieldErrors.Invalid("Options", "Only a select field has options."));
        }
        else
        {
            options = [];
        }

        definition.Update(command.Label.Trim(), command.Required, options, command.ShowInList);
        await store.SaveChangesAsync(cancellationToken);
        return CustomFieldDto.From(definition);
    }
}
