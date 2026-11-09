using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Directory;

public sealed record ListDirectoryTeamsQuery : IQuery<Result<IReadOnlyList<DirectoryTeamDto>>>;
