﻿using CA.Common.SeedWork;
using CA.MediatR.Behaviors;
using CA.MediatR.Events;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CA.MediatR
{
    public static class Extensions
    {
        /// <summary>
        /// Add the MediatR dependencies to the Asp.Net DI container
        /// </summary>
        /// <typeparam name="T">The type contained in the required lookup assembly</typeparam>
        /// <param name="services">Reference to the service collection</param>
        /// <returns>An <see cref="IServiceCollection"/> instance.</returns>
        public static IServiceCollection AddMediatRServices<T>(this IServiceCollection services, bool isTransactional = false)
        {
            services.AddScoped<IDomainEventService, DomainEventService>();
            services.AddMediatR(typeof(T).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            if (isTransactional)
            {
                services.AddTransient(typeof(IPipelineBehavior<,>), typeof(EFTransactionBehavior<,>));
            }

            AssemblyScanner.FindValidatorsInAssemblyContaining<T>().ForEach(pair =>
            {
                services.Add(ServiceDescriptor.Transient(pair.InterfaceType, pair.ValidatorType));
            });

            return services;
        }

        internal static string GetTypeName(this object @object)
        {
            return @object.GetType().Name;
        }
    }
}
