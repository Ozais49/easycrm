namespace Cem.SharedKernel;

/// <summary>
/// Lets generic code (e.g. pipeline behaviors) build a failed result without knowing the concrete type.
/// </summary>
public interface IFailable<TSelf> where TSelf : IFailable<TSelf>
{
    static abstract TSelf Failure(Error error);
}
