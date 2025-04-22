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
             * 
             * BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3902)
             * 11th Gen Intel Core i7-11850H 2.50GHz, 1 CPU, 16 logical and 8 physical cores
             * .NET SDK 10.0.100-preview.3.25201.16
             *   [Host]     : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
             *   DefaultJob : .NET 9.0.4 (9.0.425.16305), X64 RyuJIT AVX-512F+CD+BW+DQ+VL+VBMI
             * 
             * 
             * | Method                                     | Mean     | Error    | StdDev   |
             * |------------------------------------------- |---------:|---------:|---------:|
             * | SystemDateTime_Now                         | 63.70 ns | 1.101 ns | 1.840 ns |
             * | DateTimeProvider_Now                       | 65.90 ns | 1.299 ns | 1.945 ns |
             * | DateTimeProvider_Typed_Now                 | 64.78 ns | 1.168 ns | 2.135 ns |
             * | DateTimeProvider_WithContext_Now           | 65.54 ns | 1.104 ns | 1.618 ns |
             * | DateTimeProvider_Typed_WithContext_Now     | 64.89 ns | 0.956 ns | 0.799 ns |
             * |                                            |          |          |          |
             * | SystemDateTime_AddYear                     | 70.38 ns | 1.015 ns | 0.900 ns |
             * | DateTimeProvider_AddYear                   | 73.64 ns | 1.476 ns | 2.116 ns |
             * | DateTimeProvider_Typed_AddYear             | 74.68 ns | 1.398 ns | 1.373 ns |
             * | DateTimeProvider_WithContext_AddYear       | 72.23 ns | 1.470 ns | 1.444 ns |
             * | DateTimeProvider_Typed_WithContext_AddYear | 73.25 ns | 1.461 ns | 2.000 ns |
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
