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
            currentIndex = i;
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
        using var contextSequence = new DateTimeProviderContext(i => i switch
        {
            0 => new DateTime(year + 10, 5, 26),
            1 => new DateTime(year + 11, 5, 27),
            _ => DateTime.MinValue,
        });

        Assert.Equal(year + 10, DateTimeProvider.Today.Year);    // Sequence 0
        Assert.Equal(year + 11, DateTimeProvider.Today.Year);    // Sequence 1
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
        {
            return DateTimeProvider.Today.Year;
        }
    }
}

