using Cem.SharedKernel;
using Mediator;

namespace Cem.Identity.Application.Members;

public sealed record ListMembersQuery(int Page, int PageSize) : IQuery<Result<PagedResult<MemberDto>>>;
