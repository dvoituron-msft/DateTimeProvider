using System.Collections.Immutable;

/// <summary>
/// Represents the context for the <see cref="DateTimeProvider" />.
/// This context returns the specified date and time while it is in scope.
/// </summary>
public record DateTimeProviderContext : IDisposable
{
    private static readonly AsyncLocal<ImmutableStack<DateTimeProviderContext>> _asyncScopeStack = new();

    /// <summary>
    /// Gets the current <see cref="DateTimeProviderContext" />.
    /// </summary>
    internal static DateTimeProviderContext? Current => _asyncScopeStack.Value?.IsEmpty == false ? _asyncScopeStack.Value.Peek() : null;

    /// <summary>
    /// Create a new context for the <see cref="DateTimeProvider" /> using a sequence of date and time.
    /// </summary>
    /// <param name="sequence">Sequence of date and time to return while in scope.</param>
    public DateTimeProviderContext(Func<uint, DateTime> sequence)
    {
        _asyncScopeStack.Value = (_asyncScopeStack.Value ?? []).Push(this);
        Sequence = sequence;
    }

    /// <summary>
    /// Create a new context for the <see cref="DateTimeProvider" /> using the specified date and time.
    /// </summary>
    /// <param name="value">Specifies the date and time to return while in scope.</param>
    public DateTimeProviderContext(DateTime value) : this(_ => value) { }

    /// <summary>
    /// Create a new context for the <see cref="DateTimeProvider" /> using a list of date and time.
    /// Each call to <see cref="DateTimeProvider.Now" /> will return the next date and time in the list,
    /// until the last date and time is reached.
    /// If more calls are made after the last date and time, an <see cref="InvalidOperationException" /> is thrown.
    /// </summary>
    /// <param name="values"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public DateTimeProviderContext(DateTime[] values) 
        : this(i => i < values.Length 
                  ? values[i]
                  : throw new InvalidOperationException("This is the last call in the sequence. No more dates are available.")) { }

    /// <summary>
    /// Gets the current index.
    /// </summary>
    private uint CurrentIndex { get; set; }

    /// <summary>
    /// Gets the date and time to return while in scope.
    /// </summary>
    internal Func<uint, DateTime> Sequence { get; }

    /// <summary>
    /// Returns the next number between 0 and 99999999.
    /// </summary>
    /// <returns></returns>
    internal DateTime NextValue()
    {
        var value = Sequence.Invoke(CurrentIndex);

        CurrentIndex++;

        if (CurrentIndex >= 99999999)
        {
            CurrentIndex = 0;
        }

        return value;
    }

    /// <summary>
    /// Disposes the <see cref="DateTimeProviderContext" />
    /// and return to the previous context.
    /// </summary>
    public void Dispose()
    {
        if (_asyncScopeStack.Value?.IsEmpty == false)
        {
            _asyncScopeStack.Value = _asyncScopeStack.Value.Pop();
        }
    }
}
