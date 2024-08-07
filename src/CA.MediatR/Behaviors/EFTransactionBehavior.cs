﻿using CA.Common.EF;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace CA.MediatR.Behaviors
{
    /// <summary>
    /// Provide a transactional context for the current request.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request being processed.</typeparam>
    /// <typeparam name="TResponse">The type of the response generated buy tje current request.</typeparam>
    public class EFTransactionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
        where TResponse : IRequestResult
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<EFTransactionBehavior<TRequest, TResponse>> _logger;

        public EFTransactionBehavior(
            IHttpContextAccessor httpContextAccessor,
            ILogger<EFTransactionBehavior<TRequest, TResponse>> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            // check if the request needs to be tranasctional
            // this will remove the transaction overhead when
            // using query request
            if (request is ITransactionalRequest)
            {
                var response = default(TResponse);
                var typeName = request.GetTypeName();

                // get a dbcontext instance through dependency container and not
                // injected through the constructor as it will not be required for
                // query requests
                var context = _httpContextAccessor.HttpContext.RequestServices.GetRequiredService<ITransactionalDbContext>();
                try
                {
                    if (context.HasActiveTransaction)
                    {
                        return await next();
                    }

                    var strategy = context.CreateExecutionStrategy();

                    await strategy.ExecuteAsync(async () =>
                    {
                        using var transaction = await context.BeginTransactionAsync();
                        using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                        {
                            _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transaction.TransactionId, typeName, request);

                            response = await next();

                            if (response.Success)
                            {

                                _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transaction.TransactionId, typeName);

                                await context.CommitTransactionAsync(transaction);
                            }
                        }
                    });

                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "----- ERROR Handling transaction for {CommandName} ({@Command})",
                        typeName,
                        request is ISecuritySensitive<TRequest> sensitive ? sensitive.GetSafeCopy() : request);
                    throw;
                }
            }

            return await next();
        }
    }
}
