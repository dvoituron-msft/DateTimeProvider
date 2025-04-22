using BenchmarkDotNet.Attributes;

namespace SampleConsole
{
    public class DateTimeBenchmarks
    {
        int _yearToAdd = 0;

        public DateTimeBenchmarks() => _yearToAdd = new Random().Next(-100, 100);

        [Benchmark]
        public DateTime SystemDateTime_Now() => DateTime.Now;

        [Benchmark]
        public DateTime DateTimeProvider_Now() => DateTimeProvider.Now;

        [Benchmark]
        public DateTime DateTimeProvider_Typed_Now() => DateTimeProvider<MyClass>.Now;

        [Benchmark]
        public DateTime DateTimeProvider_WithContext_Now() => DateTimeProvider.WithContext("MyFile1.cs").Now;

        [Benchmark]
        public DateTime DateTimeProvider_Typed_WithContext_Now() => DateTimeProvider<MyClass>.WithContext("MyFile1.cs").Now;

        [Benchmark]
        public DateTime SystemDateTime_AddYear() => DateTime.Now.AddYears(_yearToAdd);

        [Benchmark]
        public DateTime DateTimeProvider_AddYear() => DateTimeProvider.Now.AddYears(_yearToAdd);

        [Benchmark]
        public DateTime DateTimeProvider_Typed_AddYear() => DateTimeProvider<MyClass>.Now.AddYears(_yearToAdd);

        [Benchmark]
        public DateTime DateTimeProvider_WithContext_AddYear() => DateTimeProvider.WithContext("MyFile2.cs").Now.AddYears(_yearToAdd);

        [Benchmark]
        public DateTime DateTimeProvider_Typed_WithContext_AddYear() => DateTimeProvider<MyClass>.WithContext("MyFile2.cs").Now.AddYears(_yearToAdd);
    }

    public record MyClass { }
}
