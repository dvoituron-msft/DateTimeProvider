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
        public DateTime SystemDateTime_AddYear() => DateTime.Now.AddYears(_yearToAdd);

        [Benchmark]
        public DateTime DateTimeProvider_AddYear() => DateTimeProvider.Now.AddYears(_yearToAdd);
    }
}
