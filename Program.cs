namespace ConsoleDateTimeProvider
{
    internal class Program
    {
        /// <summary>
        /// Bulk examples to test how DateTimeProvider works.
        /// </summary>
        /// <returns></returns>
        static async Task Main()
        {
            // No Context
            Console.WriteLine($"TDAY: {DateTimeProvider.Today:yyyy}");

            // Synchronous
            using (var context1 = new DateTimeProviderContext(new DateTime(2000, 5, 26)))
            {
                Console.WriteLine($"2000: {DateTimeProvider.Today:yyyy}");

                using (var context2 = new DateTimeProviderContext(new DateTime(2001, 01, 01)))
                {
                    Console.WriteLine($"2001: {DateTimeProvider.Today:yyyy}");
                }

                Console.WriteLine($"2000: {DateTimeProvider.Today:yyyy}");
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
            Console.WriteLine($"TDAY: {DateTimeProvider.Today:yyyy}");
        }

        private static async Task DisplayThreadAsync(int year, bool? withAwait = null)
        {
            using (var context = new DateTimeProviderContext(new DateTime(year, 01, 01)))
            {
                if (withAwait == true)
                {
                    Console.WriteLine($"{year}: {DateTimeProvider.Today:yyyy}");
                    await DisplayThreadAsync(year + 1);
                    Console.WriteLine($"{year}: {DateTimeProvider.Today:yyyy}");
                }

                else if (withAwait == false)
                {
                    Console.WriteLine($"{year}: {DateTimeProvider.Today:yyyy}");
                    DisplayThreadAsync(year + 1);
                    Console.WriteLine($"{year}: {DateTimeProvider.Today:yyyy}");
                }

                else
                {
                    Console.WriteLine($"{year}: {DateTimeProvider.Today:yyyy}");
                }
            }

            await Task.CompletedTask;
        }
    }
}
