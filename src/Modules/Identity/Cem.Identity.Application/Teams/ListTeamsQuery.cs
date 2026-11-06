using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Teams;

public sealed record ListTeamsQuery(int Page, int PageSize) : IQuery<Result<PagedResult<TeamDto>>>;
