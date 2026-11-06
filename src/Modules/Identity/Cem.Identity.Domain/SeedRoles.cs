using Cem.SharedKernel.Permissions;
using static Cem.SharedKernel.Permissions.PermissionLevel;

namespace Cem.Identity.Domain;

/// <summary>
/// The roles every new tenant starts with. Read it next to the table in docs/product/roles-and-permissions.md:
/// one block per column, one argument per row.
/// </summary>
public static class SeedRoles
{
    public const string AdminName = "Admin";
    public const string ViewerName = "Viewer";

    public static IReadOnlyList<Role> CreateAll() =>
    [
        Role.CreateSystem(AdminName, Matrix(
            crm: Everything(All), calendar: Everything(All), sales: Everything(All), billing: Everything(All),
            projects: Everything(All), portfolio: Everything(All), inbox: Everything(All), settings: Everything(All),
            pricing: FieldAccess.ReadWrite, financial: FieldAccess.ReadWrite, contactPii: FieldAccess.ReadWrite)),

        Role.Create("Manager", Matrix(
            crm: Everything(All), calendar: Everything(All), sales: Everything(All), billing: Everything(All),
            projects: Everything(All), portfolio: Everything(All), inbox: Everything(All), settings: Grant.Nothing,
            pricing: FieldAccess.ReadWrite, financial: FieldAccess.ReadOnly, contactPii: FieldAccess.ReadWrite)),

        Role.Create("Sales", Matrix(
            crm: Everything(PermissionLevel.Team), calendar: Everything(Own), sales: Everything(PermissionLevel.Team), billing: Everything(Own),
            projects: ReadOnly(All), portfolio: ReadOnly(All), inbox: Everything(PermissionLevel.Team), settings: Grant.Nothing,
            pricing: FieldAccess.ReadWrite, financial: FieldAccess.None, contactPii: FieldAccess.ReadWrite)),

        Role.Create("Creative/Staff", Matrix(
            crm: ReadOnly(Own), calendar: Everything(Own), sales: Grant.Nothing, billing: Grant.Nothing,
            projects: Everything(PermissionLevel.Team), portfolio: new Grant(All, All, All, None), inbox: Grant.Nothing, settings: Grant.Nothing,
            pricing: FieldAccess.None, financial: FieldAccess.None, contactPii: FieldAccess.None)),

        Role.Create(ViewerName, Matrix(
            crm: ReadOnly(All), calendar: ReadOnly(All), sales: Grant.Nothing, billing: Grant.Nothing,
            projects: ReadOnly(All), portfolio: ReadOnly(All), inbox: Grant.Nothing, settings: Grant.Nothing,
            pricing: FieldAccess.None, financial: FieldAccess.None, contactPii: FieldAccess.None))
    ];

    private static Grant Everything(PermissionLevel level) => new(level, level, level, level);

    private static Grant ReadOnly(PermissionLevel level) => new(None, level, None, None);

    private static PermissionMatrix Matrix(
        Grant crm, Grant calendar, Grant sales, Grant billing, Grant projects, Grant portfolio, Grant inbox, Grant settings,
        FieldAccess pricing, FieldAccess financial, FieldAccess contactPii) =>
        new(
            new Dictionary<Scope, IReadOnlyDictionary<PermissionAction, PermissionLevel>>
            {
                [Scope.Crm] = crm.ToLevels(),
                [Scope.Calendar] = calendar.ToLevels(),
                [Scope.Sales] = sales.ToLevels(),
                [Scope.Billing] = billing.ToLevels(),
                [Scope.Projects] = projects.ToLevels(),
                [Scope.Portfolio] = portfolio.ToLevels(),
                [Scope.Inbox] = inbox.ToLevels(),
                [Scope.Settings] = settings.ToLevels()
            },
            new Dictionary<FieldGroup, FieldAccess>
            {
                [FieldGroup.Pricing] = pricing,
                [FieldGroup.Financial] = financial,
                [FieldGroup.ContactPii] = contactPii
            });

    /// <summary>The level for each action of one scope.</summary>
    private readonly record struct Grant(PermissionLevel Create, PermissionLevel Read, PermissionLevel Edit, PermissionLevel Delete)
    {
        public static Grant Nothing => new(None, None, None, None);

        public IReadOnlyDictionary<PermissionAction, PermissionLevel> ToLevels() => new Dictionary<PermissionAction, PermissionLevel>
        {
            [PermissionAction.Create] = Create,
            [PermissionAction.Read] = Read,
            [PermissionAction.Edit] = Edit,
            [PermissionAction.Delete] = Delete
        };
    }
}
