# Project 1: AI Chat API

## 🎯 Overview

A **production-ready ASP.NET Core 8 Web API** that provides real-time chat capabilities powered by **Google Gemini AI** via **Semantic Kernel**.

This project demonstrates:
- ✅ **Clean Architecture** (Domain, Application, Infrastructure, Presentation layers)
- ✅ **SOLID Principles** (Dependency Inversion, Single Responsibility)
- ✅ **Semantic Kernel** orchestration for AI integration
- ✅ **Provider-agnostic design** (swap Gemini → OpenAI in configuration only)
- ✅ **Production-ready** error handling, logging, validation
- ✅ **Conversation context** management (multi-turn conversations)
- ✅ **Comprehensive unit tests** (11 tests, 100% core service coverage)

---

## 🏗️ Architecture

### Layer Structure

```
Project1.ChatApi/
├── Domain/
│   └── ChatMessage.cs                    # ✓ Core business entity
│
├── Application/
│   ├── Interfaces/
│   │   └── IChatService.cs               # ✓ Contract (what, not how)
│   ├── DTOs/
│   │   ├── ChatRequest.cs                # ✓ API input model
│   │   ├── ChatResponse.cs               # ✓ API output model
│   │   └── ChatHistoryResponse.cs        # ✓ History model
│   └── Services/
│
├── Infrastructure/
│   ├── AI/
│   │   └── SemanticKernelChatService.cs  # ✓ Gemini implementation
│   └── Configuration/
│       └── SemanticKernelConfiguration.cs # ✓ DI & Kernel setup
│
├── Presentation/
│   ├── Controllers/
│   │   └── ChatController.cs             # ✓ REST API endpoints
│   └── Models/
│
├── Program.cs                             # ✓ Startup & DI configuration
├── appsettings.json                      # ✓ Public configuration
└── appsettings.Development.json          # ✓ Local secrets (gitignored)
```

### Layer Responsibilities

| Layer | Responsibility | Example |
|-------|---|---|
| **Domain** | Pure business logic, NO external dependencies | `ChatMessage` entity class |
| **Application** | Use cases, interfaces, DTOs | `IChatService` interface, request/response models |
| **Infrastructure** | External service implementations | `SemanticKernelChatService` calls Google Gemini API |
| **Presentation** | API endpoints, HTTP concerns | `ChatController` REST endpoints with validation |

### Why This Architecture?

- **Testability** - Easy to mock infrastructure layer
- **Maintainability** - Clear separation of concerns
- **Scalability** - Projects 2-7 reuse these patterns
- **Flexibility** - Swap Gemini for OpenAI without changing business logic

---

## 🚀 Getting Started

### Prerequisites

- **.NET 8 SDK** - [Download](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Google Gemini API Key** - [Get Free Key](https://ai.google.dev)

### Setup Instructions

#### 1️⃣ Clone & Navigate

```bash
git clone <your-repo-url>
cd AI-Learning-Lab/Project1.ChatApi
```

#### 2️⃣ Configure API Key (Secure)

**Option A: User Secrets (Recommended for Development)**

```bash
# Initialize User Secrets for this project
dotnet user-secrets init

# Set your Gemini API key securely
dotnet user-secrets set "GoogleAI:ApiKey" "your-api-key-here"

# Verify it's set
dotnet user-secrets list
```

Benefits:
- ✅ Secrets stored outside your project directory
- ✅ Can't accidentally commit secrets
- ✅ Automatically merged into configuration during development

**Option B: Environment Variables**

```bash
# Windows PowerShell
$env:GoogleAI__ApiKey="your-api-key-here"

# Linux/Mac Bash
export GoogleAI__ApiKey="your-api-key-here"
```

**Option C: appsettings.Development.json** (Less secure - local only)

Create `appsettings.Development.json`:
```json
{
  "GoogleAI": {
    "ApiKey": "your-api-key-here"
  }
}
```

⚠️ **Add to .gitignore** to prevent accidental commits

#### 3️⃣ Run the Application

```bash
dotnet run

# Expected output:
# Now listening on: https://localhost:7236
# Now listening on: http://localhost:5193
# Application started. Press Ctrl+C to shut down.
```

#### 4️⃣ Test via Swagger UI

Open in browser: **https://localhost:7236/swagger/index.html**

You'll see interactive API documentation where you can test endpoints.

---

## 📚 API Endpoints

### 1. Send Chat Message

**Endpoint:** `POST /api/chat/send`

**Description:** Send a message and get AI response with conversation context

**Request Body:**
```json
{
  "message": "Explain machine learning in detail",
  "chatSessionId": null
}
```

**Parameters:**
- `message` (required): Your message to the AI
- `chatSessionId` (optional): Existing session ID. If null, creates new session.

**Response (200 OK):**
```json
{
  "chatSessionId": "550e8400-e29b-41d4-a716-446655440000",
  "userMessage": "Explain machine learning in detail",
  "assistantMessage": "Machine learning is a branch of AI that...",
  "timestamp": "2026-07-18T13:40:00Z"
}
```

**Error Response (400 Bad Request):**
```json
{
  "error": "Message cannot be empty"
}
```

**Error Response (503 Service Unavailable):**
```json
{
  "error": "AI service is unavailable. Please try again later."
}
```

---

### 2. Get Chat History

**Endpoint:** `GET /api/chat/history/{chatSessionId}`

**Description:** Retrieve all messages from a conversation session

**URL Parameters:**
- `chatSessionId` (required): The session ID to retrieve history for

**Response (200 OK):**
```json
{
  "chatSessionId": "550e8400-e29b-41d4-a716-446655440000",
  "messages": [
    {
      "id": "msg-001",
      "role": "user",
      "content": "What is machine learning?",
      "createdAt": "2026-07-18T13:40:00Z"
    },
    {
      "id": "msg-002",
      "role": "assistant",
      "content": "Machine learning is a subset of AI that...",
      "createdAt": "2026-07-18T13:40:10Z"
    },
    {
      "id": "msg-003",
      "role": "user",
      "content": "Tell me more about supervised learning",
      "createdAt": "2026-07-18T13:41:00Z"
    },
    {
      "id": "msg-004",
      "role": "assistant",
      "content": "Supervised learning is a type of machine learning where...",
      "createdAt": "2026-07-18T13:41:15Z"
    }
  ]
}
```

**Error Response (404 Not Found):**
```json
{
  "error": "Chat session not found"
}
```

---

### 3. Clear Chat History

**Endpoint:** `DELETE /api/chat/clear/{chatSessionId}`

**Description:** Delete all messages from a conversation session

**URL Parameters:**
- `chatSessionId` (required): The session ID to clear

**Response (204 No Content)**

No response body - just confirms deletion.

---

## 🧪 Testing

### Run All Tests

```bash
cd Project1.ChatApi.Tests

# Run tests with summary
dotnet test

# Run with detailed output
dotnet test -v d

# Run specific test
dotnet test --filter "ClassName=SemanticKernelChatServiceTests"
```

### Test Coverage

**11 Unit Tests:**

| Category | Tests | What It Tests |
|----------|-------|---|
| **Constructor** | 2 | Dependency injection validation |
| **SendMessage** | 4 | Input validation, error handling, session creation |
| **GetHistory** | 2 | Query validation, empty results |
| **ClearHistory** | 2 | Clear operation, validation |
| **Integration** | 1 | Service initialization |

### Expected Output

```
Test Run Successful.
Total tests: 11
     Passed: 11
     Failed: 0
```

---

## 🔄 How It Works

### Conversation Flow

```
User sends: "What is AI?"
    ↓
[ChatController] validates HTTP request
    ↓
[IChatService.SendMessageAsync] processes business logic
    ↓
[SemanticKernelChatService] calls Gemini
    ↓
[Semantic Kernel] orchestrates the call
    ↓
[Google Gemini API] generates response
    ↓
Response flows back up through layers
    ↓
Returns JSON to user: { assistantMessage: "AI is...", chatSessionId: "..." }
```

### Context Management

The service maintains **conversation memory** by:

1. **Storing all messages** in a static dictionary (keyed by `chatSessionId`)
2. **Building complete history** before each Gemini call
3. **Passing full context** to Gemini (all previous messages)
4. **Enabling multi-turn conversations** (follow-up questions understand context)

**Example:**

```
Turn 1:
  User: "What is machine learning?"
  Gemini sees: [System prompt] + [User: "What is machine learning?"]
  Response: "Machine learning is a subset of AI..."

Turn 2:
  User: "Give me an example"
  Gemini sees: [System prompt] + [Turn 1 exchange] + [User: "Give me an example"]
  Response: "For example, email spam filters use ML..." 
  ✓ Knows context from Turn 1!
```

---

## 🔧 Configuration

### appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "GoogleAI": {
    "ApiKey": ""  // Set via User Secrets or environment variables
  }
}
```

### Switching AI Providers

**To use OpenAI instead of Gemini:**

1. **Update `SemanticKernelConfiguration.cs`:**

```csharp
// Remove
.AddGoogleAIGeminiChatCompletion(...)

// Add
.AddOpenAIChatCompletion(
    modelId: "gpt-4o-mini",
    apiKey: configuration["OpenAI:ApiKey"])
```

2. **Update `appsettings.json`:**

```json
{
  "OpenAI": {
    "ApiKey": "your-openai-key"
  }
}
```

**That's it!** The rest of your code works unchanged. This is the power of dependency inversion.

---

## 🛠️ Development

### Adding a New Feature

**Example: Add sentiment analysis**

1. **Update Domain** (`Domain/ChatMessage.cs`):
```csharp
public string Sentiment { get; set; }  // "positive", "negative", "neutral"
```

2. **Update Interface** (`Application/Interfaces/IChatService.cs`):
```csharp
Task<string> AnalyzeSentimentAsync(string message);
```

3. **Implement** (`Infrastructure/AI/SemanticKernelChatService.cs`):
```csharp
public async Task<string> AnalyzeSentimentAsync(string message)
{
    // Your implementation
}
```

4. **Add API Endpoint** (`Presentation/Controllers/ChatController.cs`):
```csharp
[HttpPost("analyze-sentiment")]
public async Task<IActionResult> AnalyzeSentiment([FromBody] string message)
{
    var sentiment = await _chatService.AnalyzeSentimentAsync(message);
    return Ok(new { sentiment });
}
```

5. **Write Tests** (`Project1.ChatApi.Tests/Services/`):
```csharp
[Fact]
public async Task AnalyzeSentiment_PositiveMessage_ReturnPositive()
{
    var result = await _fixture.ChatService.AnalyzeSentimentAsync("Great day!");
    Assert.Equal("positive", result);
}
```

---

## 📊 Key Design Decisions

| Decision | Rationale |
|----------|-----------|
| **Clean Architecture** | Testable, maintainable, scales to Projects 2-7 |
| **Dependency Inversion** | Depend on interfaces, not concrete classes |
| **In-memory Storage** | Fast for learning (Projects 5+ will use SQL Server) |
| **Semantic Kernel** | Unified API across Gemini, OpenAI, etc. |
| **Static Dictionary** | Simple session management for Project 1 |

---

## ⚠️ Limitations & Future Improvements

### Current Limitations

1. **In-memory Storage** - Data lost when app restarts
   - Solution: Add SQL Server persistence (Project 5)

2. **Single-instance** - Doesn't scale across multiple servers
   - Solution: Move to distributed cache (Redis) + SQL Server

3. **No Authentication** - Anyone can call the API
   - Solution: Add JWT authentication (future project)

4. **No Rate Limiting** - No protection against abuse
   - Solution: Add API rate limiting middleware

---

## 🐛 Troubleshooting

### Issue: "API key not configured"

**Error Message:**
```
Google Gemini API key is not configured. Please add 'GoogleAI:ApiKey' to your User Secrets or appsettings.json
```

**Solution:**
```bash
dotnet user-secrets set "GoogleAI:ApiKey" "your-key"
```

---

### Issue: "Model not found"

**Error Message:**
```
models/gemini-1.5-flash is not found for API version v1beta
```

**Solution:** Try these models in order:
- `gemini-3.5-flash` (fastest, recommended)
- `gemini-2.0-flash`
- `gemini-1.5-pro`

Update in `SemanticKernelConfiguration.cs`:
```csharp
modelId: "gemini-3.5-flash"
```

---

### Issue: "Quota exceeded"

**Error Message:**
```
Quota exceeded for metric: generativelanguage.googleapis.com/generate_content_free_tier_requests
```

**Solutions:**
1. Wait until tomorrow (free tier resets daily)
2. Enable billing on [Google Cloud Console](https://console.cloud.google.com)
3. Use a different API key

---

### Issue: "Response truncated"

If responses are cut short, the issue is usually token limits. Ensure `SemanticKernelConfiguration.cs` doesn't limit output tokens too aggressively.

---

## 📖 Learning Resources

- **Semantic Kernel** - [Official Docs](https://learn.microsoft.com/en-us/semantic-kernel/)
- **Google Gemini API** - [Developer Guide](https://ai.google.dev/docs)
- **Clean Architecture** - [Uncle Bob's Article](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- **SOLID Principles** - [Wikipedia](https://en.wikipedia.org/wiki/SOLID)
- **ASP.NET Core** - [Microsoft Docs](https://learn.microsoft.com/en-us/aspnet/core)

---

## 🗺️ Project Roadmap

- ✅ **Project 1:** AI Chat API (You are here)
- 🔜 **Project 2:** AI Email Generator
- 🔜 **Project 3:** SQL Assistant
- 🔜 **Project 4:** Code Reviewer
- 🔜 **Project 5:** Resume Analyzer
- 🔜 **Project 6:** AI Agent
- 🔜 **Project 7:** Chat with PDF using RAG

Each project builds on the architecture and patterns learned in Project 1.

---

## 📄 License

MIT License - See LICENSE file

---

## 🤝 Contributing

This is a learning project. Contributions and suggestions welcome!

---

**Built with ❤️ for AI Engineering Learning**
