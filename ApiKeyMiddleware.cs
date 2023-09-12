public class ApiKeyMiddleware {
    private readonly RequestDelegate _next;
    private
    const string APIKEY = "XApiKey";
    public ApiKeyMiddleware(RequestDelegate next) {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context) {
        if (!context.Request.Headers.TryGetValue(APIKEY, out
                var extractedApiKey)) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("The Api Key for accessing this endpoint is not available");
            return;
        }
        var appSettings = context.RequestServices.GetRequiredService < IConfiguration > ();
        var apiKey = appSettings.GetValue < string > (APIKEY);
        if (!apiKey.Equals(extractedApiKey)) {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("The authentication key is incorrect : Unauthorized access");
            return;
        }
        await _next(context);
    }
}