namespace Cem.SharedKernel;

public abstract class Entity
{
    /// <summary>UUID v7: time-ordered, so inserts stay index-friendly.</summary>
    public Guid Id { get; protected init; } = Guid.CreateVersion7();
}
