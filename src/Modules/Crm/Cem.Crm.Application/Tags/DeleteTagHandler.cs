using Cem.Crm.Application.Abstractions;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Crm.Application.Tags;

/// <summary>Hard delete; the foreign keys remove the tag from every company and contact.</summary>
public sealed class DeleteTagHandler(ICrmStore store) : ICommandHandler<DeleteTagCommand, Result>
{
    public async ValueTask<Result> Handle(DeleteTagCommand command, CancellationToken cancellationToken)
    {
        var tag = await store.FindTagAsync(command.Id, cancellationToken);
        if (tag is null)
        {
            return Result.Failure(CrmErrors.TagNotFound);
        }

        store.Remove(tag);
        await store.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
