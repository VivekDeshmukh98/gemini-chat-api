using Microsoft.Extensions.DependencyInjection;

namespace Project1.ChatApi.Tests
{
    // Minimal startup for Xunit.DependencyInjection
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            // No special services required for these unit tests
        }
    }
}
