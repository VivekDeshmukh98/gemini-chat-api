using Project1.ChatApi.Features.Chat.Services;
using Project1.ChatApi.Tests.Fixtures;
using Xunit;

namespace Project1.ChatApi.Tests.Services
{
    public class SemanticKernelChatServiceTests : IClassFixture<SemanticKernelChatServiceFixture>
    {
        private readonly SemanticKernelChatServiceFixture _fixture;

        public SemanticKernelChatServiceTests(SemanticKernelChatServiceFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
        }

        // Test 1: Constructor with null kernel
        [Fact]
        public void Constructor_NullKernel_ThrowsArgumentNullException()
        {
            var logger = _fixture.MockLogger.Object;
            Assert.Throws<ArgumentNullException>(
                () => new SemanticKernelChatService(null, logger));
        }

        // Test 2: Constructor with null logger
        [Fact]
        public void Constructor_NullLogger_ThrowsArgumentNullException()
        {
            var kernel = _fixture.Kernel;    
            Assert.Throws<ArgumentNullException>(
                () => new SemanticKernelChatService(kernel, null));
        }

        // Test 3: Constructor with valid dependencies creates instance
        [Fact]
        public void Constructor_ValidDependencies_CreatesService()
        {
            Assert.NotNull(_fixture.ChatService);
        }

        // Test 4: SendMessage with empty string throws
        [Fact]
        public async Task SendMessage_EmptyMessage_Throws()
        {
            var sessionId = Guid.NewGuid().ToString();
            await Assert.ThrowsAsync<ArgumentException>(
                () => _fixture.ChatService.SendChatMessageAsync("", sessionId));
        }

        // Test 5: SendMessage with null throws
        [Fact]
        public async Task SendMessage_NullMessage_Throws()
        {
            var sessionId = Guid.NewGuid().ToString();
            await Assert.ThrowsAsync<ArgumentException>(
                () => _fixture.ChatService.SendChatMessageAsync(null, sessionId));
        }

        // Test 6: SendMessage with whitespace throws
        [Fact]
        public async Task SendMessage_WhitespaceMessage_Throws()
        {
            var sessionId = Guid.NewGuid().ToString();
            await Assert.ThrowsAsync<ArgumentException>(
                () => _fixture.ChatService.SendChatMessageAsync("   ", sessionId));
        }

        // Test 7: GetHistory with empty session ID throws
        [Fact]
        public async Task GetHistory_EmptySessionId_Throws()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _fixture.ChatService.GetChatHistoryAsync(""));
        }

        // Test 8: GetHistory with null session ID throws
        [Fact]
        public async Task GetHistory_NullSessionId_Throws()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _fixture.ChatService.GetChatHistoryAsync(null));
        }

        // Test 9: GetHistory for non-existent session returns empty
        [Fact]
        public async Task GetHistory_NonExistentSession_ReturnsEmpty()
        {
            var sessionId = Guid.NewGuid().ToString();
            var history = await _fixture.ChatService.GetChatHistoryAsync(sessionId);
            Assert.Empty(history);
        }

        // Test 10: ClearHistory with empty session ID throws
        [Fact]
        public async Task ClearHistory_EmptySessionId_Throws()
        {
            await Assert.ThrowsAsync<ArgumentException>(
                () => _fixture.ChatService.ClearChatHistoryAsync(""));
        }

        // Test 11: ClearHistory with valid session completes
        [Fact]
        public async Task ClearHistory_ValidSession_Succeeds()
        {
            var sessionId = Guid.NewGuid().ToString();
            await _fixture.ChatService.ClearChatHistoryAsync(sessionId);
        }
    }
}