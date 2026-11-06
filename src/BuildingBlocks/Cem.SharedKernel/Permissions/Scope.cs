namespace Cem.SharedKernel.Permissions;

/// <summary>An area of the product that roles grant access to; one row of the permission matrix.</summary>
public enum Scope
{
    /// <summary>Companies and Contacts.</summary>
    Crm,
    Calendar,

    /// <summary>Leads and Deals.</summary>
    Sales,

    /// <summary>Quotes and Invoices.</summary>
    Billing,

    /// <summary>Projects and Deliverables.</summary>
    Projects,
    Portfolio,
    Inbox,

    /// <summary>Settings, including user, role and team administration.</summary>
    Settings
}
