/// <summary>
/// Returns the current date and time on this computer, expressed as the local time.
/// </summary>
public static class DateTimeProvider
{
    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time 
    /// on this computer, expressed as the local time.
    /// </summary>
    public static DateTime Now => DateTimeProviderContext.Current == null
                                ? DateTime.Now
                                : DateTimeProviderContext.Current.NextValue();

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to the current date and time
    /// on this computer, expressed as the Coordinated Universal Time (UTC).
    /// </summary>
    public static DateTime UtcNow => Now.ToUniversalTime();

    /// <summary>
    /// Gets a <see cref="DateTime" /> object that is set to today's date, with the time component set to 00:00:00.
    /// </summary>
    public static DateTime Today => Now.Date;
}
