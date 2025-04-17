/// <summary>
/// Returns the current date and time on this computer, expressed as the local time.
/// </summary>
public static class DateTimeProvider
{
    public static CurrentContext WithContext(object context) => new(context);

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time 
    /// on this computer, expressed as the local time.
    /// </summary>
    public static DateTime Now => GetNow();

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time
    /// on this computer, expressed as the Coordinated Universal Time (UTC).
    /// </summary>
    public static DateTime UtcNow => GetNow().ToUniversalTime();

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to today's date, with the time component set to 00:00:00.
    /// </summary>
    public static DateTime Today => GetNow().Date;

    /// <summary>
    /// Indicates whether a context is required to be active.
    /// </summary>
    public static bool RequiredActiveContext { get; set; }

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time 
    /// on this computer, expressed as the local time.
    /// </summary>
    internal static DateTime GetNow(object? context = null) => DateTimeProviderContext.Current == null
                                                            ? GetSystemDate()
                                                            : DateTimeProviderContext.Current.NextValue(context);

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

    /// <summary>
    /// Returns the contextual current date and time on this computer, expressed as the local time.
    /// </summary>
    public record CurrentContext
    {
        private object _context;

        internal CurrentContext(object context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets a <see cref="DateTime" /> object that is set to the current date and time 
        /// on this computer, expressed as the local time.
        /// </summary>
        public DateTime Now => GetNow(_context);

        /// <summary>
        /// Gets a <see cref="DateTime" /> object that is set to the current date and time
        /// on this computer, expressed as the Coordinated Universal Time (UTC).
        /// </summary>
        public DateTime UtcNow => GetNow(_context).ToUniversalTime();

        /// <summary>
        /// Gets a <see cref="DateTime" /> object that is set to today's date, with the time component set to 00:00:00.
        /// </summary>
        public DateTime Today => GetNow(_context).Date;

    }
}
