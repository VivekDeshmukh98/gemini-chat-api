using Project1.ChatApi.Application.Interfaces;
using Project1.ChatApi.Infrastructure.AI;
using Project1.ChatApi.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);




// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


// Add services to the container

// 1. Add Semantic Kernel with Google Gemini
// This extension method is defined in SemanticKernelConfiguration.cs
builder.Services.AddSemanticKernelWithGemini(builder.Configuration);

// 2. Register the chat service implementation
// When something asks for IChatService, give them SemanticKernelChatService
// This follows the Dependency Inversion principle (SOLID)
builder.Services.AddScoped<IChatService, SemanticKernelChatService>();

// 3. Add standard ASP.NET Core services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 4. Add CORS (so frontend can call this API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// 5. Add logging
builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();
