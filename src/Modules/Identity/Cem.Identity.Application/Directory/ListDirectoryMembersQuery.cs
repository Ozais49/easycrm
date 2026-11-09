using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Directory;

public sealed record ListDirectoryMembersQuery : IQuery<Result<IReadOnlyList<DirectoryMemberDto>>>;
