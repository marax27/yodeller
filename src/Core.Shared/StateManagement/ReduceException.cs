namespace Core.Shared.StateManagement;

/// <summary>
/// Wraps an exception thrown during reducer invocation.
/// </summary>
public class ReduceException : Exception
{
    public ReduceException(string message, Exception innerException)
    : base(message, innerException)
    {
        
    }
}
