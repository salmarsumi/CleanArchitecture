using CA.Common.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text.Json;

namespace CA.Common.Middleware
{
    public class ExceptionHandler
    {
        public static Action<IApplicationBuilder> Handler =>
            (exceptionHandlerApp) =>
            {
                exceptionHandlerApp.Run(async (context) =>
                {
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                    if (exceptionHandlerPathFeature?.Error is not null)
                    {
                        var logger = context.RequestServices.GetRequiredService<ILogger<ExceptionHandler>>();
                        var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
                        var correlationId = Guid.Empty.ToString();
                        if (context.Request.Headers.ContainsKey(Constants.CORRELATION_HEADER))
                        {
                            correlationId = context.Request.Headers[Constants.CORRELATION_HEADER].First();
                        }

                        using (var scope = logger.BeginScope("{CorrelationId}", correlationId))
                        {
                            logger.LogError(exceptionHandlerPathFeature.Error, exceptionHandlerPathFeature.Error.Message);
                        }

                        JsonErrorResponse result;
                        switch (exceptionHandlerPathFeature.Error)
                        {
                            case ValidationException exception:
                                result = new JsonErrorResponse
                                {
                                    ExceptionType = "Validation",
                                    Data = exception.Errors?.Select(error => error.ErrorMessage)?.ToArray()
                                };
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                                break;
                            case NotFoundException notfoundException:
                                result = new JsonErrorResponse
                                {
                                    ExceptionType = "Not Found",
                                    Key = notfoundException.Key,
                                    Name = notfoundException.Name
                                };
                                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                                break;
                            case ConcurrencyException concurrencyException:
                                result = new JsonErrorResponse
                                {
                                    ExceptionType = "Concurrency"
                                };
                                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                                break;
                            default:
                                result = new JsonErrorResponse
                                {
                                    ExceptionType = "Generic"
                                };

                                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                                break;
                        }

                        if (env.IsDevelopment())
                        {
                            result.DeveloperMessage = exceptionHandlerPathFeature.Error.ToString();
                        }

                        await context.Response.WriteAsJsonAsync(result);
                    }

                });
            };

        private class JsonErrorResponse
        {
            public string ExceptionType { get; set; }
            public object Key { get; set; }
            public string Name { get; set; }
            public string DeveloperMessage { get; set; }
            public object Data { get; set; }
        }
    }
}
