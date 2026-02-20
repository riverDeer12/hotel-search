using System.Text;

namespace HotelSearch.API.Middlewares;

public class AdditionalRequestLogging
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdditionalRequestLogging> _logger;

    public AdditionalRequestLogging(RequestDelegate next, ILogger<AdditionalRequestLogging> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Processing request at {@DateTimeUTC}",
            DateTime.UtcNow
        );

        context.Request.EnableBuffering(); // Allows us to read the body multiple times

        if (context.Request.Method != "GET")
        {
            var requestBody = await new StreamReader(context.Request.Body, Encoding.UTF8).ReadToEndAsync();

            context.Request.Body.Position = 0; // Reset stream position for the next reader

            _logger.LogInformation($"Request Body: {requestBody}");
        }

        await _next(context);

        _logger.LogInformation($"Request Completed");
    }
}