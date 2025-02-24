using Xunit;
using Xunit.Abstractions;

public class DateTimeProviderUnitTests : StrictAutoMockTestClass
{
    public DateTimeProviderUnitTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    [Fact]
    public void LibWithContext()
    {
        using var context = new DateTimeProviderContext(new DateTime(2020, 5, 26));

        var year = SampleLib.MyClass.GetCurrentYear();

        Assert.Equal(2020, year);
    }

    [Fact]
    public void LibWithoutContext()
    {
        Assert.Throws<InvalidOperationException>(() =>
        {
            var year = SampleLib.MyClass.GetCurrentYear();
        });
    }

    [Theory]
    [MemberData(nameof(GetNumbers))]
    public void MySimpleTest(int year)
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
    public void MyTestSequence(int year)
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
    public void MyTestUsingListOfDates(int year)
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

    // List of numbers from 1 to 100
    public static IEnumerable<object[]> GetNumbers()
    {
        for (int i = 1; i <= 100; i++)
        {
            yield return new object[] { i };
        }
    }
}

