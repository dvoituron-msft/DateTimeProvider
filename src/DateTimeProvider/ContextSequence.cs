/// <summary>
/// Context information for a sequence.
/// </summary>
/// <typeparam name="U">The type parameter for the context sequence.</typeparam>
/// <param name="Index">The index of the context sequence.</param>
/// <param name="SourceContext">The source context associated with the sequence.</param>
/// <param name="SourceType">The type of the source.</param>
public record ContextSequence<U>(uint Index, object? SourceContext, Type SourceType);