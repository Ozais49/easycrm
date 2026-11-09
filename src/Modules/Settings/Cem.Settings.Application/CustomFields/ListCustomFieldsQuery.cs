using Cem.Settings.Domain;
using Cem.SharedKernel;
using Mediator;

namespace Cem.Settings.Application.CustomFields;

public sealed record ListCustomFieldsQuery(CustomFieldEntity EntityType) : IQuery<Result<IReadOnlyList<CustomFieldDto>>>;
