# AI-Learning-Lab

A multi-project .NET 8 solution demonstrating AI-powered workflows, including a chat API and Semantic Kernel based services for email generation and conversational agents.

This README gives an overview of the repository, how to configure and run the projects locally, and notes about the architecture and contribution guidelines.

---

## Contents

- Project1.ChatApi — ASP.NET Core Web API providing chat and email-generation endpoints
- (Other projects) — add additional project descriptions here if present

---

## Key Features

- Provider-agnostic AI integration using Semantic Kernel
- Example service for email generation (SemanticKernelEmailService)
- Clean, testable architecture (separation of Application, Infrastructure, Presentation)
- Configuration-driven secrets and provider settings

---

## Prerequisites

- .NET 8 SDK (download from https://dotnet.microsoft.com)
- An LLM provider account and API key (OpenAI, Google Gemini, or another provider)
- Recommended: Visual Studio 2022/2026, Visual Studio Code, or your preferred editor

---

## Setup & Configuration

1. Clone the repository

   ```bash
   git clone https://github.com/VivekDeshmukh98/gemini-chat-api.git
   cd AI-Learning-Lab
   ```

2. Restore and build

   ```bash
   dotnet restore
   dotnet build
   ```

3. Configure secrets and provider credentials

   The projects expect keys and endpoints for your LLM provider. Configure them via one of the following methods:

   - dotnet user-secrets (recommended for development)
   - environment variables
   - appsettings.Development.json (local only, not recommended for committed files)

   Example user-secrets keys (adjust to your actual configuration keys used in the project):

   ```bash
   dotnet user-secrets init
   dotnet user-secrets set "LLM:ApiKey" "<your-api-key>"
   dotnet user-secrets set "LLM:Endpoint" "<optional-endpoint>"
   ```

   Or set environment variables (PowerShell):

   ```powershell
   $env:LLM__ApiKey = "<your-api-key>"
   ```

4. Update appsettings.json or project configuration if you use different keys or providers.

---

## Running the API

From the solution root run the primary API project:

```bash
dotnet run --project Project1.ChatApi
```

Open the Swagger UI (if enabled) or hit the exposed endpoints with curl/Postman. Check the project's launch settings for exact URLs and ports.

---

## Testing

If this solution contains test projects, run them with:

```bash
dotnet test
```

Use Visual Studio Test Explorer for interactive test running and debugging.

---

## Project layout (high level)

- Project1.ChatApi/
  - Features/
    - Email/
      - Services/SemanticKernelEmailService.cs — orchestrates Semantic Kernel prompts and calls to LLMs to produce email content
  - Controllers/ — API controllers and endpoints
  - Program.cs, appsettings.json — application boot and configuration

Adjust the structure description to match additional projects in this repo as needed.

---

## Folder structure (detailed)

Below is a suggested folder layout and files an interviewer would expect to see. Paths are relative to the Project1.ChatApi project root.

- /Presentation
  - /Controllers — API controllers (e.g., ChatController.cs, EmailController.cs)
  - /Models — request/response models used by controllers
- /Application
  - /Interfaces — application-level contracts (e.g., IChatService, IEmailService)
  - /DTOs — data transfer objects used across layers
  - /Services — use-case services that implement business logic orchestration
- /Domain
  - Entities and value objects (e.g., ChatMessage.cs, EmailDraft.cs)
- /Infrastructure
  - /AI — implementations that call Semantic Kernel or LLM providers (SemanticKernelEmailService.cs, SemanticKernelChatService.cs)
  - /Persistence — data stores or in-memory repositories for chat history
  - /Configuration — DI registration and kernel/provider setup
- /Tests
  - Unit and integration tests (Project1.ChatApi.Tests)
- Program.cs, appsettings.json, appsettings.Development.json, launchSettings.json

Place prompt templates, resource files, and sample data under a /Resources or /Templates folder so reviewers can quickly inspect prompt engineering.

---

## Quick review guide — what to look for

When exploring this repository, the sections below will help you quickly understand the design, components, and how to run or extend the project.

- Architecture & layering
  - Confirm the separation between Presentation, Application, Infrastructure, and Domain layers. This improves maintainability and testability.

- Dependency injection & configuration
  - Inspect Program.cs and DI registration code to see how interfaces map to implementations and how configuration (appsettings/user-secrets) is used.

- Semantic Kernel & LLM provider integration
  - Check Infrastructure/AI for Semantic Kernel setup and LLM client usage. Providers and keys should be abstracted and read from configuration.

- Prompt templates & resources
  - Look for a Templates or Resources folder containing prompt templates, examples, and any sample data used for prompt engineering.

- Error handling & logging
  - Verify controllers and services use structured error handling, proper HTTP status codes, and ILogger for observability.

- Tests
  - Review unit tests for deterministic logic (prompt composition, DTO mapping) and any integration tests that exercise real flows.

- Security & secrets
  - Ensure API keys and secrets are loaded from user-secrets or environment variables and not committed to source control.

- API surface & documentation
  - Confirm Swagger/OpenAPI is enabled and that controllers use clear models and endpoints for predictable behavior.

- Observability & health
  - Check for health checks, structured logging, and any telemetry hooks that help diagnose runtime issues.

- Extensibility & provider-agnostic design
  - Confirm that provider-specific logic sits behind interfaces so additional LLM providers can be added with minimal changes.

- Performance considerations
  - Review async usage, request timeouts for LLM calls, caching strategy, and any bulk/request batching logic.

## Files and locations to inspect first

To get up to speed quickly, open these files and folders in this order:

1. Project1.ChatApi/Program.cs — startup, DI registrations, and middleware
2. Project1.ChatApi/Presentation/Controllers — API endpoints (ChatController, EmailController)
3. Project1.ChatApi/Features/Email/Services/SemanticKernelEmailService.cs — core prompt orchestration and LLM calls
4. Project1.ChatApi/Infrastructure/AI — Semantic Kernel and provider integrations
5. Project1.ChatApi/Domain — entities and value objects (ChatMessage, EmailDraft)
6. Project1.ChatApi/Tests — unit and integration tests
7. appsettings.json and appsettings.Development.json — configuration and local overrides

These files give a clear picture of how requests flow through the system, where prompts are composed, and how external providers are invoked.

If the above areas are implemented cleanly, the project is well-structured, easy to extend, and straightforward to run locally.

---

## SemanticKernelEmailService (notes for maintainers)

- Purpose: Compose context-aware email subject and body using prompt templates and Semantic Kernel orchestration.
- Responsibilities:
  - Build prompt templates and context
  - Call the configured LLM provider through a configured client
  - Return structured results (subject, body, metadata)
- Recommendations:
  - Keep templates in configuration or resource files
  - Add unit tests for prompt generation and deterministic logic
  - Isolate provider-specific code behind interfaces so providers can be swapped easily

---

## Development guidelines

- Follow existing folder and naming conventions used by Project1.ChatApi
- Add unit tests for any non-trivial logic
- Keep secrets out of source control; use user-secrets or env vars for keys
- Use small, focused commits with clear messages

---

## Contributing

1. Fork the repository
2. Create a branch for your feature/bugfix
3. Add tests for new behavior
4. Open a Pull Request with a clear description of changes

---

## License

Add a LICENSE file to explicitly state the repository license. If you are not sure, consider MIT or Apache-2.0 for permissive licensing.

---

## Need help?

Open an issue in the repository describing the problem, desired change, or question.
