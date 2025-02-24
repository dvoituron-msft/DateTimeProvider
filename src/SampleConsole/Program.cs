namespace SampleConsole
{
    using BenchmarkDotNet.Running;
    using SampleLib;

    /* ***********************************************************************
     *  Update the RUN_BENCHMARKS constant (in csproj) to run the benchmarks.
     * ***********************************************************************
     */

    public class Program
    {
#if RUN_BENCHMARKS
        static void Main()
        {
            /*
             * BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3321)
             * 11th Gen Intel Core i7-11850H 2.50GHz, 1 CPU, 16 logical and 8 physical cores
             * .NET SDK 9.0.200-preview.0.25057.12
             *   [Host]     : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
             *   DefaultJob : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
             * 
             * 
             * | Method                   | Mean     | Error    | StdDev   | Median   |
             * |------------------------- |---------:|---------:|---------:|---------:|
             * | SystemDateTime_Now       | 69.06 ns | 1.774 ns | 5.174 ns | 68.09 ns |
             * | DateTimeProvider_Now     | 68.78 ns | 1.896 ns | 5.561 ns | 67.79 ns |
             * | SystemDateTime_AddYear   | 74.82 ns | 1.683 ns | 4.936 ns | 73.90 ns |
             * | DateTimeProvider_AddYear | 75.25 ns | 2.002 ns | 5.582 ns | 73.16 ns |
             * 
             */

            // Run benchmarks: dotnet run -c Release
            var summary = BenchmarkRunner.Run<DateTimeBenchmarks>();
            return;
        }
#else

        /// <summary>
        /// Bulk examples to test how DateTimeProvider works.
        /// </summary>
        /// <returns></returns>
        static async Task Main()
        {


            // No Context
            Console.WriteLine($"TDAY: {MyClass.GetCurrentYear()}");

            // Synchronous
            using (var context1 = new DateTimeProviderContext(new DateTime(2000, 5, 26)))
            {
                Console.WriteLine($"2000: {MyClass.GetCurrentYear()}");

                using (var context2 = new DateTimeProviderContext(new DateTime(2001, 01, 01)))
                {
                    Console.WriteLine($"2001: {MyClass.GetCurrentYear()}");
                }

                Console.WriteLine($"2000: {MyClass.GetCurrentYear()}");
            }

            // Asynchronous
            await DisplayThreadAsync(2005);
            await DisplayThreadAsync(2006);

            var t1 = DisplayThreadAsync(2010);
            var t2 = DisplayThreadAsync(2011, withAwait: true);
            var t3 = DisplayThreadAsync(2013, withAwait: false);
            await DisplayThreadAsync(2007);
            var t4 = DisplayThreadAsync(2015);

            await Task.WhenAll(t1, t2, t3, t4);

            // Not Context
            Console.WriteLine($"TDAY: {MyClass.GetCurrentYear()}");
        }

        private static async Task DisplayThreadAsync(int year, bool? withAwait = null)
        {
            using (var context = new DateTimeProviderContext(new DateTime(year, 01, 01)))
            {
                if (withAwait == true)
                {
                    Console.WriteLine($"{year}: {MyClass.GetCurrentYear()}");
                    await DisplayThreadAsync(year + 1);
                    Console.WriteLine($"{year}: {MyClass.GetCurrentYear()}");
                }

                else if (withAwait == false)
                {
                    Console.WriteLine($"{year}: {MyClass.GetCurrentYear()}");
                    DisplayThreadAsync(year + 1);
                    Console.WriteLine($"{year}: {MyClass.GetCurrentYear()}");
                }

                else
                {
                    Console.WriteLine($"{year}: {MyClass.GetCurrentYear()}");
                }
            }

            await Task.CompletedTask;
        }
#endif
    }
}
