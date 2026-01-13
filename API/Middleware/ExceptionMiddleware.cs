using System;
using System.Net;
using System.Text.Json;
using API.Errors;

namespace API.Middleware;

// RequestDelegate: pass the request on to after we've check if there is an error
// ILogger: can change the type of the login system that we are using, needs to have a type <ExceptionMiddleware>
// IHostEnvironment: check if we are in dev mode or production mode
public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
{
  // InvokeAsync: middleware components must have this method
  public async Task InvokeAsync(HttpContext context)
  {
    try
    {
      // since this is an exception middleware, we just pass the context
      await next(context);
    }
    catch (Exception ex)
    {
      // since logger does not like ex.Message directly, we use {message} as a placeholder
      logger.LogError(ex, "{message}", ex.Message);
      context.Response.ContentType = "application/json"; // format we want to return
      context.Response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500 error

      // take the exception and return it as a json formated response
      var response = env.IsDevelopment()
        ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace) // detailed error in dev mode
        : new ApiException(context.Response.StatusCode, ex.Message, "Internal Server Error");

      // api controllers respect json formating conventions, so we need to set the naming policy to camel case
      var options = new JsonSerializerOptions
      {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
      };

      var json = JsonSerializer.Serialize(response, options);

      await context.Response.WriteAsync(json);
    }
  }
}
