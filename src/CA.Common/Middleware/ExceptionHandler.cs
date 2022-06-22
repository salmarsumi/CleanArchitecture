using CA.Common.Exceptions;
using CA.Common.ResponseTypes;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;

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
                            case ConcurrencyException concurrencyException:
                                result = new JsonErrorResponse
                                {
                                    ExceptionType = "Concurrency"
                                };
                                context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                                break;
                            case AntiforgeryValidationException:
                                result = new JsonErrorResponse
                                {
                                    ExceptionType = "Validation"
                                };
                                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
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
    }
}
