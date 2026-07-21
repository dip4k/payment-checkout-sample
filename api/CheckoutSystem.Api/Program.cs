using CheckoutSystem.Api.Endpoints.V1;
using CheckoutSystem.Api.Middleware;
using CheckoutSystem.Application;
using CheckoutSystem.Infrastructure;
using CheckoutSystem.Infrastructure.Persistence;
using Microsoft.OpenApi;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

const string UiDevelopmentCorsPolicy = "UiDevelopment";

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Checkout System API",
        Version = "v1",
        Description = "Versioned checkout endpoints for catalogue lookup, order calculation, and idempotent order submission.",
    });
    options.OperationFilter<CheckoutSwaggerOperationFilter>();

    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy(UiDevelopmentCorsPolicy, policy =>
    {
        policy
            .WithOrigins("http://localhost:4173", "http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

await DatabaseInitializer.InitializeAsync(app.Services, app.Lifetime.ApplicationStopping);

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.DocumentTitle = "Checkout System API";
        options.RoutePrefix = "swagger";
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Checkout System API v1");
    });
    app.UseCors(UiDevelopmentCorsPolicy);
}

app.UseHttpsRedirection();
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.MapCheckoutEndpoints();

app.Run();
