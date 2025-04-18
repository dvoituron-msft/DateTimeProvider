public class DateTimeProvider
{
    public static CurrentContext WithContext(object context) => new(context);

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time 
    /// on this computer, expressed as the local time.
    /// </summary>
    public static DateTime Now => GetNow<object>(null);

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time
    /// on this computer, expressed as the Coordinated Universal Time (UTC).
    /// </summary>
    public static DateTime UtcNow => GetNow<object>(null).ToUniversalTime();

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to today's date, with the time component set to 00:00:00.
    /// </summary>
    public static DateTime Today => GetNow<object>(null).Date;

    /// <summary>
    /// Indicates whether a context is required to be active.
    /// </summary>
    public static bool RequiredActiveContext { get; set; }

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time 
    /// on this computer, expressed as the local time.
    /// </summary>
    internal static DateTime GetNow<U>(object? context = null)
        => DateTimeProviderContext<U>.Current == null
         ? GetSystemDate()
         : DateTimeProviderContext<U>.Current.NextValue<U>(context);

    /// <summary>
    /// Returns the current date and time on this computer.
    /// </summary>
    /// <param name="requiredContext">Indicates whether a context is required to be active (used by internal unit tests).</param>
    /// <returns>The current date and time on this computer.</returns>
    /// <exception cref="InvalidOperationException">If <see cref="RequiredActiveContext"/> is true and no context is active.</exception>
    internal static DateTime GetSystemDate(bool requiredContext = true)
    {
        if (RequiredActiveContext && requiredContext)
        {
            throw new InvalidOperationException("DateTimeProvider requires a context to be set (e.g. `using var context = new DateTimeProviderContext(new DateTime(2025, 1, 18));`");
        }
        else
        {
            return DateTime.Now;
        }
    }
}

/// <summary>
/// Returns the current date and time on this computer, expressed as the local time.
/// </summary>
public class DateTimeProvider<T>
{
    public static CurrentContext<object> WithContext(object context) => new(context);

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time 
    /// on this computer, expressed as the local time.
    /// </summary>
    public static DateTime Now => DateTimeProvider.GetNow<T>();

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time
    /// on this computer, expressed as the Coordinated Universal Time (UTC).
    /// </summary>
    public static DateTime UtcNow => DateTimeProvider.GetNow<T>().ToUniversalTime();

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to today's date, with the time component set to 00:00:00.
    /// </summary>
    public static DateTime Today => DateTimeProvider.GetNow<T>().Date;
}
