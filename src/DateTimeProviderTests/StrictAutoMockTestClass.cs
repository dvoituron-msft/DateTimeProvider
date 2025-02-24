
using Xunit.Abstractions;

public abstract class StrictAutoMockTestClass : IDisposable
{
    protected StrictAutoMockTestClass(ITestOutputHelper output)
    {
        DateTimeProvider.RequiredActiveContext = true;
    }

    public void Dispose()
    {
        
    }
}
