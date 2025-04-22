public class DateTimeProviderSampleTests
{
    [Fact]
    public void DateTimeProviderSample_Single()
    {
        // Register a single context
        using var context = new DateTimeProviderContext(new DateTime(2020, 5, 26));  // 2nd quarter

        // Call the method
        var quarter = MyClass.Quarter;

        Assert.Equal(2, quarter);
    }

    [Fact]
    public void DateTimeProviderSample_Typed()
    {
        // Register 2 contexts of different types
        using var context1 = new DateTimeProviderContext<MyClass1>(new DateTime(2020, 9, 26));  // 3rd quarter
        using var context2 = new DateTimeProviderContext<MyClass2>(new DateTime(2024, 2, 26));  // 1st quarter

        // Call a method from each class
        var quarter1 = MyClass1.Quarter;
        var quarter2 = MyClass2.Quarter;

        Assert.Equal(3, quarter1);
        Assert.Equal(1, quarter2);
    }

    [Fact]
    public void DateTimeProviderSample_Combined()
    {
        // Register the MyClass1 context
        using var context1 = new DateTimeProviderContext<MyClass1>(new DateTime(2020, 9, 26));  // 3rd quarter

        // Call the main class method, which uses the MyClass1 quarter.
        var previous = MyClass.GetPreviousQuarter();

        Assert.Equal(2, previous);
    }

    [Fact]
    public void DateTimeProvider_WithContext()
    {
        // Context Sequence
        using var contextSequence = new DateTimeProviderContext(
            seq => seq.SourceContext switch
            {
                "From_Class_1" => new DateTime(2011, 5, 26),
                "From_Class_2" => new DateTime(2022, 5, 27),
                _ => DateTime.MinValue,
            });

        Assert.Equal(2011, MyClass1.GetYear());
        Assert.Equal(2022, MyClass2.GetYear()); 
    }
}

internal class MyClass
{
    public static int Quarter => (DateTimeProvider.Now.Month - 1) / 3 + 1;
    public static int GetPreviousQuarter() => MyClass1.Quarter > 1 ? MyClass1.Quarter - 1 : 4;
}

internal class MyClass1
{
    public static int Quarter => (DateTimeProvider<MyClass1>.Now.Month - 1) / 3 + 1;
    public static int GetYear() => DateTimeProvider.WithContext("From_Class_1").Now.Year;
}

internal class MyClass2
{
    public static int Quarter => (DateTimeProvider<MyClass2>.Now.Month - 1) / 3 + 1;
    public static int GetYear() => DateTimeProvider.WithContext("From_Class_2").Now.Year;
}
