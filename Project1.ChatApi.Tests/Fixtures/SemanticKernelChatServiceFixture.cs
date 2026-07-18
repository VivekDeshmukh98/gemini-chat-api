using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Moq;
using Project1.ChatApi.Infrastructure.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.ChatApi.Tests.Fixtures
{
    /// <summary>
    /// Test fixture that provides mocked dependencies for testing SemanticKernelChatService.
    /// 
    /// Why use a fixture?
    /// - Reusable setup across multiple tests
    /// - Consistent mock configuration
    /// - Easier to maintain tests when behavior changes
    /// </summary>
    public class SemanticKernelChatServiceFixture : IDisposable
    {
        // Kernel instance for tests
        public Kernel Kernel { get; private set; }
        public Mock<ILogger<SemanticKernelChatService>> MockLogger { get; private set; }

        // The service we're testing
        public SemanticKernelChatService ChatService { get; private set; }

        public SemanticKernelChatServiceFixture()
        {
            // Create a lightweight kernel instance for tests
            Kernel = Kernel.CreateBuilder().Build();
            MockLogger = new Mock<ILogger<SemanticKernelChatService>>();

            // Initialize the service with the kernel instance and mock logger
            ChatService = new SemanticKernelChatService(
                Kernel,
                MockLogger.Object);
        }

        public void Dispose()
        {
            // Cleanup if needed
            GC.SuppressFinalize(this);
        }
    }
}

