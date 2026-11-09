using Cem.Crm.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Tags;

public sealed class UpdateTagHandler(ICrmStore store) : ICommandHandler<UpdateTagCommand, Result<TagDto>>
{
    public async ValueTask<Result<TagDto>> Handle(UpdateTagCommand command, CancellationToken cancellationToken)
    {
        var tag = await store.FindTagAsync(command.Id, cancellationToken);
        if (tag is null)
        {
            return Result<TagDto>.Failure(CrmErrors.TagNotFound);
        }

        var name = command.Name.Trim();
        if (await store.TagNameTakenAsync(name, tag.Id, cancellationToken))
        {
            return Result<TagDto>.Failure(CrmErrors.TagNameTaken);
        }

        tag.Update(name, command.Color);

        // A concurrent request may have taken the name between the check and the save.
        if (!await store.TrySaveAsync(cancellationToken))
        {
            return Result<TagDto>.Failure(CrmErrors.TagNameTaken);
        }

        return TagDto.From(tag);
    }
}
