using api_gateway_microsservice.Extensions;
using api_gateway_microsservice.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Extensions
builder.Services
    .AddReverseProxyConfiguration(builder.Configuration)
    .AddJwtAuthentication(builder.Configuration)
    .AddAuthorizationPolicies();


builder.Services.AddHealthChecks();

var app = builder.Build();

// Middlewares
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new
        {
            error = "Gateway internal error"
        });
    });
});
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/health");

// YARP
app.MapReverseProxy();

app.Run();