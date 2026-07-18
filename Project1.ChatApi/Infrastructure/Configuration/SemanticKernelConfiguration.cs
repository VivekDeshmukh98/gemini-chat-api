using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Project1.ChatApi.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration for Semantic Kernel setup.
    /// This handles creating and configuring the Kernel with Gemini.
    /// 
    /// Why separate this?
    /// - Keeps Program.cs clean (SOLID: Single Responsibility)
    /// - Reusable across projects
    /// - Easy to swap providers by changing this one class
    /// </summary>
    public static class SemanticKernelConfiguration
    {
        /// <summary>
        /// Add Semantic Kernel with Google Gemini to dependency injection.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The configuration object (from appsettings.json)</param>
        /// <returns>The service collection (for method chaining)</returns>
        public static IServiceCollection AddSemanticKernelWithGemini(this IServiceCollection services, IConfiguration configuration)
        {
            // Get the Gemini API key from configuration
            var geminiApiKey = configuration["GoogleAI:ApiKey"];
            if (string.IsNullOrEmpty(geminiApiKey))
            {
                throw new InvalidOperationException("Gemini API key is not configured. Please set it in appsettings.json or environment variables.");
            }

            // Tell the compiler to ignore the experimental warning for the Google connector
#pragma warning disable SKEXP0070

            var kernel = Kernel.CreateBuilder()
                .AddGoogleAIGeminiChatCompletion(
                    modelId: "gemini-3.5-flash",
                    apiKey: geminiApiKey)
                .Build();

#pragma warning restore SKEXP0070

            services.AddSingleton(kernel);

            return services;
        }
    }
}