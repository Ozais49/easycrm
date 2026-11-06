namespace Cem.SharedKernel.Permissions;

/// <summary>A group of sensitive fields with its own read and edit permission.</summary>
public enum FieldGroup
{
    /// <summary>Product prices, deal amounts, quote line prices.</summary>
    Pricing,

    /// <summary>Invoice totals, payments, project budget, revenue widgets.</summary>
    Financial,

    /// <summary>Phone, email and address of Contacts and Leads.</summary>
    ContactPii
}
