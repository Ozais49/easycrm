namespace Cem.SharedKernel;

public interface ISoftDeletable
{
    DateTimeOffset? DeletedAt { get; }
}
