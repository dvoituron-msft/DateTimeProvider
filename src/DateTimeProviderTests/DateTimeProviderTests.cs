using Xunit;
using Xunit.Abstractions;

public class DateTimeProviderTests : StrictAutoMockTestClass
{
    public DateTimeProviderTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void DateTimeProvider_WithContext()
    {
        using var context = new DateTimeProviderContext(new DateTime(2020, 5, 26));

        var year = MyUserClass.GetCurrentYear();

        Assert.Equal(2020, year);
    }

    [Fact]
    public void DateTimeProvider_WithoutContext()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var year = MyUserClass.GetCurrentYear();
        });

        Assert.Throws<InvalidOperationException>(() =>
        {
            var year = MyUserClass.GetCurrentYear_OfInt();
        });
    }

    [Fact]
    public void DateTimeProvider_SystemDate_WithRequiredContext()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var date = DateTimeProvider.GetSystemDate(requiredContext: true);
        });
    }

    [Fact]
    public void DateTimeProvider_SystemDate_WithoutRequiredContext()
    {
        var date = DateTimeProvider.GetSystemDate(requiredContext: false);

        Assert.Equal(DateTime.Today.Year, date.Date.Year);
    }

    [Fact]
    public void DateTimeProvider_DisposeEmptyContext()
    {
        using var context = new DateTimeProviderContext(new DateTime(2020, 5, 26));

        context.Dispose();
        context.Dispose();
    }

    [Fact]
    public void DateTimeProvider_UtcNow()
    {
        var date = new DateTime(2020, 5, 26);
        var currentOffset = Math.Abs(new DateTimeOffset(date).Offset.TotalHours);

        using var context = new DateTimeProviderContext(date);
        var contextOffset = Math.Abs((DateTimeProvider.UtcNow - DateTimeProvider.Now).TotalHours);

        Assert.Equal(currentOffset, contextOffset);
    }

    [Fact]
    public void DateTimeProvider_ResetCurrentIndex()
    {
        const uint maxValue = uint.MaxValue;

        var currentIndex = 0u;
        using var context = new DateTimeProviderContext(i =>
        {
            currentIndex = i.Index;
            return new DateTime(2020, 5, 26);
        });

        context.ForceNextValue(maxValue);

        // First call => Max value
        _ = DateTimeProvider.Today;
        Assert.Equal(maxValue, currentIndex);

        // Second call => Reset
        _ = DateTimeProvider.Today;
        Assert.Equal(0u, currentIndex);
    }

    [Theory]
    [MemberData(nameof(GetNumbers))]
    public void DateTimeProvider_SimpleTest(int year)
    {
        // Context 1
        using var context1 = new DateTimeProviderContext(new DateTime(year, 5, 26));
        Assert.Equal(year, DateTimeProvider.Today.Year);

        using (var context2 = new DateTimeProviderContext(new DateTime(year + 1, 5, 26)))
        {
            // Context 2
            Assert.Equal(year + 1, DateTimeProvider.Today.Year);
        }

        // Context 1
        Assert.Equal(year, DateTimeProvider.Today.Year);
    }

    [Theory]
    [MemberData(nameof(GetNumbers))]
    public void DateTimeProvider_Sequence(int year)
    {
        // Context Sequence
        using var contextSequence = new DateTimeProviderContext(i => i.Index switch
        {
            0 => new DateTime(year + 10, 5, 26),
            1 => new DateTime(year + 11, 5, 27),
            _ => DateTime.MinValue,
        });

        Assert.Equal(year + 10, DateTimeProvider.Today.Year);    // Sequence 0
        Assert.Equal(year + 11, DateTimeProvider.Today.Year);    // Sequence 1
    }

    [Fact]
    public void DateTimeProvider_CallingContext()
    {
        const int year = 2020;

        // Context Sequence
        using var contextSequence = new DateTimeProviderContext(
            seq => seq.SourceContext switch
            {
                "File1.cs" => new DateTime(year + 10, 5, 26),
                "File2.cs" => new DateTime(year + 11, 5, 27),
                "File3.cs" => new DateTime(year + 12, 5, 27),
                _ => DateTime.MinValue,
            });

        Assert.Equal(year + 10, DateTimeProvider.WithContext("File1.cs").Today.Year);    // Sequence 0
        Assert.Equal(year + 11, DateTimeProvider.WithContext("File2.cs").Now.Year);      // Sequence 1
        Assert.Equal(year + 12, DateTimeProvider.WithContext("File3.cs").UtcNow.Year);   // Sequence 2
    }

    [Theory]
    [MemberData(nameof(GetNumbers))]
    public void DateTimeProvider_GenericSource(int year)
    {
        // Context Sequence
        using var contextUnused1 = new DateTimeProviderContext<string>(new DateTime(year + 99, 5, 26));   // Not used
        using var contextUnused2 = new DateTimeProviderContext<string>(new DateTime(year + 99, 5, 26));   // Not used
        using var context1 = new DateTimeProviderContext<int>(new DateTime(year + 10, 5, 26));
        using var context2 = new DateTimeProviderContext<MyUserClass>(new DateTime(year + 11, 5, 26));

        Assert.Equal(year + 11, MyUserClass.GetCurrentYear_MyUserClass());     // Context 2
        Assert.Equal(year + 10, MyUserClass.GetCurrentYear_OfInt());           // Context 1
    }

    [Fact]
    public void DateTimeProvider_EmptyTypedContext()
    {
        // Arrange
        Func<ContextSequence<EmptyTypedContext>, DateTime> sequence = _ => DateTime.Now;
        var emptyTypedContext = new EmptyTypedContext();

        // Act
        var context = new DateTimeProviderContext(sequence);

        // Assert
        Assert.IsAssignableFrom<DateTimeProviderContext<EmptyTypedContext>>(context);
        Assert.NotNull(context);
        Assert.IsType<EmptyTypedContext>(emptyTypedContext);
    }

    [Fact]
    public void DateTimeProvider_GenericSource_Sequence()
    {
        int year = 2020;

        // Context Sequence
        using var contextSequence = new DateTimeProviderContext<int>(
            seq => seq.SourceType switch
            {
                Type t when t == typeof(int) => new DateTime(year + 10, 5, 26),
                Type t when t == typeof(string) => new DateTime(year + 11, 5, 27),
                _ => DateTime.MinValue,
            });

        //Assert.Equal(year + 11, MyUserClass.GetCurrentYearMyUserClass());     // Context 2
        Assert.Equal(year + 10, MyUserClass.GetCurrentYear_OfInt());           // Context 1
    }

    [Theory]
    [MemberData(nameof(GetNumbers))]
    public void DateTimeProvider_UsingListOfDates(int year)
    {
        // Context Sequence
        using var contextSequence = new DateTimeProviderContext(
        [
            new DateTime(year + 10, 5, 26),
            new DateTime(year + 11, 5, 27)
        ]);

        Assert.Equal(year + 10, DateTimeProvider.Today.Year);    // Sequence 0
        Assert.Equal(year + 11, DateTimeProvider.Today.Year);    // Sequence 1

        Assert.Throws<InvalidOperationException>(() => DateTimeProvider.Today); // No more dates are available
    }

    // List of numbers from 1 to 10
    // To simulate multiple calls to DateTimeProvider
    public static IEnumerable<object[]> GetNumbers()
    {
        for (int i = 1; i <= 10; i++)
        {
            yield return new object[] { i };
        }
    }

    private class MyUserClass
    {
        public static int GetCurrentYear()
            => DateTimeProvider.WithContext("My sample context").Now.Year;
        public static int GetCurrentYear_MyUserClass() => DateTimeProvider<MyUserClass>.Now.Year;
        public static int GetCurrentYear_OfInt() => DateTimeProvider<int>.Now.Year;
    }
}

