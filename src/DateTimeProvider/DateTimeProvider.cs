/// <summary>
/// Returns the current date and time on this computer, expressed as the local time.
/// </summary>
public class DateTimeProvider<T>
{
    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time 
    /// on this computer, expressed as the local time.
    /// </summary>
    internal static DateTime GetNow(object? context = null)
        => DateTimeProviderContext<T>.Current == null
         ? GetSystemDate()
         : DateTimeProviderContext<T>.Current.NextValue(context);

    /// <summary>
    /// Returns the current date and time on this computer.
    /// </summary>
    /// <param name="requiredContext">Indicates whether a context is required to be active (used by internal unit tests).</param>
    /// <returns>The current date and time on this computer.</returns>
    /// <exception cref="InvalidOperationException">If <see cref="RequiredActiveContext"/> is true and no context is active.</exception>
    internal static DateTime GetSystemDate(bool requiredContext = true)
    {
        if (DateTimeProvider.RequiredActiveContext && requiredContext)
        {
            var contextType = typeof(T) == typeof(DateTimeProviderContext.EmptyTypedContext) ? "" : $"<{typeof(T).Name}>";
            throw new InvalidOperationException($"DateTimeProvider requires a context{contextType} to be set (e.g. `using var context = new DateTimeProviderContext{contextType}(new DateTime(2025, 1, 18));`");
        }
        else
        {
            return DateTime.Now;
        }
    }

    /// <summary>
    /// Creates and returns a custom context to get the current date and time on this computer, expressed as the local time.
    /// </summary>
    /// <param name="context">Custom context to use in the Unit Tests</param>
    /// <returns></returns>
    public static CurrentContext<T> WithContext(object context) => new(context);

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time 
    /// on this computer, expressed as the local time.
    /// </summary>
    public static DateTime Now => DateTimeProvider<T>.GetNow();

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time
    /// on this computer, expressed as the Coordinated Universal Time (UTC).
    /// </summary>
    public static DateTime UtcNow => DateTimeProvider<T>.GetNow().ToUniversalTime();

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to today's date, with the time component set to 00:00:00.
    /// </summary>
    public static DateTime Today => DateTimeProvider<T>.GetNow().Date;

    /// <summary>
    /// Returns the contextual current date and time on this computer, expressed as the local time.
    /// </summary>
    public class CurrentContext<U>
    {
        private object? _context;

        /// <summary />
        internal CurrentContext(object? context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a <see cref="DateTime" /> object that is set to the current date and time 
        /// on this computer, expressed as the local time.
        /// </summary>
        public DateTime Now => DateTimeProvider<U>.GetNow(_context);

        /// <summary>
        /// Gets a <see cref="DateTime" /> object that is set to the current date and time
        /// on this computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime UtcNow => DateTimeProvider<U>.GetNow(_context).ToUniversalTime();

        /// <summary>
        /// Gets a <see cref="DateTime" /> object that is set to today's date, with the time component set to 00:00:00.
        /// </summary>
        public DateTime Today => DateTimeProvider<U>.GetNow(_context).Date;
    }
}

/// <summary>
/// Returns the current date and time on this computer, expressed as the local time.
/// </summary>
public class DateTimeProvider : DateTimeProvider<DateTimeProviderContext.EmptyTypedContext>
{
    /// <summary>
    /// Indicates whether a context is required to be active.
    /// </summary>
    public static bool RequiredActiveContext { get; set; }
}
