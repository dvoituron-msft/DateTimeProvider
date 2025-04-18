public class CurrentContext : CurrentContext<object>
{
    internal CurrentContext(object? context) : base(context)
    {
    }
}

/// <summary>
/// Returns the contextual current date and time on this computer, expressed as the local time.
/// </summary>
public class CurrentContext<T>
{
    private object? _context;

    internal CurrentContext(object? context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time 
    /// on this computer, expressed as the local time.
    /// </summary>
    public DateTime Now => DateTimeProvider.GetNow<T>(_context);

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time
    /// on this computer, expressed as the Coordinated Universal Time (UTC).
    /// </summary>
    public DateTime UtcNow => DateTimeProvider.GetNow<T>(_context).ToUniversalTime();

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to today's date, with the time component set to 00:00:00.
    /// </summary>
    public DateTime Today => DateTimeProvider.GetNow<T>(_context).Date;
}