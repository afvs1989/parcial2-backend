using Microsoft.AspNetCore.Antiforgery;

namespace CineApi.Api.Middleware;

public class CsrfValidationMiddleware(RequestDelegate next)
{
    private static readonly HashSet<string> MetodosMutantes = new(StringComparer.OrdinalIgnoreCase)
    {
        HttpMethods.Post, HttpMethods.Put, HttpMethods.Patch, HttpMethods.Delete
    };

    private static readonly string[] RutasExcluidas =
    [
        "/api/auth/login"
    ];

    public async Task InvokeAsync(HttpContext context, IAntiforgery antiforgery)
    {
        var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;

        if (MetodosMutantes.Contains(context.Request.Method) &&
            !RutasExcluidas.Any(r => path.StartsWith(r, StringComparison.OrdinalIgnoreCase)))
        {
            await antiforgery.ValidateRequestAsync(context);
        }

        await next(context);
    }
}

public static class CsrfValidationMiddlewareExtensions
{
    public static IApplicationBuilder UseCsrfValidation(this IApplicationBuilder app) =>
        app.UseMiddleware<CsrfValidationMiddleware>();
}
