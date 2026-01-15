using InterviewAgent.Api.Data;
using InterviewAgent.Api.Services;
using InterviewAgent.Api.Services.Ai;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// CORS for local dev
var allowedOrigins = "_allowedOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowedOrigins, policy =>
    {
        policy
            .WithOrigins("http://localhost:5177", "http://localhost:5185")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// SQLite
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// App services
builder.Services.AddScoped<InterviewService>();
builder.Services.AddSingleton<PromptLoader>();

// AI client selection:
// Start with MockAiClient (works offline). Later switch to OpenAiClient by config.
var aiProvider = builder.Configuration["Ai:Provider"] ?? "Mock";
if (aiProvider.Equals("OpenAI", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddHttpClient<OpenAiClient>();
    builder.Services.AddScoped<IAiClient, OpenAiClient>();
}
else
{
    builder.Services.AddScoped<IAiClient, MockAiClient>();
}

var app = builder.Build();

app.Lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Backend shutting down...");
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors(allowedOrigins);
app.MapControllers();

// Auto-apply migrations in dev (simple local DX)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();
