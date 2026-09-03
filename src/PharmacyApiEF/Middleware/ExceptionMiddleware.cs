namespace PharmacyApiEF.Middleware;

public class ValidationException : Exception
{
    public ValidationException(string message)
        : base(message)
    {
    }
}

public class NotFoundException : Exception
{
    public NotFoundException(string message)
        : base(message)
    {
    }
}

public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }
}

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(
        RequestDelegate next,
        ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context)
    {
        try
        {
            await _next(context);
        }

        catch (ValidationException ex)
        {
            _logger.LogWarning(ex.Message);

            context.Response.StatusCode =
                StatusCodes.Status400BadRequest;

            await context.Response.WriteAsJsonAsync(
                new
                {
                    message = ex.Message
                });
        }

        catch (NotFoundException ex)
        {
            _logger.LogWarning(ex.Message);

            context.Response.StatusCode =
                StatusCodes.Status404NotFound;

            await context.Response.WriteAsJsonAsync(
                new
                {
                    message = ex.Message
                });
        }

        catch (ConflictException ex)
        {
            _logger.LogWarning(ex.Message);

            context.Response.StatusCode =
                StatusCodes.Status409Conflict;

            await context.Response.WriteAsJsonAsync(
                new
                {
                    message = ex.Message
                });
        }

        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Unhandled exception.");

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(
                new
                {
                    message =
                        "Unexpected server error."
                });
        }
    }
}