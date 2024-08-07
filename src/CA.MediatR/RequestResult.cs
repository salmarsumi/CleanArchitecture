﻿using Microsoft.AspNetCore.Http;

namespace CA.MediatR
{
    /// <summary>
    /// A wrapper class around the request result. Implements IRequestResult.All requests passing through the 
    /// MediatR pipeline should return their result as a RequestResult.The RequestResult indicates the success 
    /// or the failure of the operation. The use of a result class adds the benefit of not relying on throwing 
    /// exceptions to interrupt the execution flow.
    /// </summary>
    /// <typeparam name="T">The type of the result generated by the current request.</typeparam>
    public class RequestResult<T> : IRequestResult
    {
        /// <summary>
        /// Private constructor to control instance initiation.
        /// </summary>
        private RequestResult() { }

        public bool Success { get; private set; }
        public bool IsNotFound { get; private set; }
        public bool IsNotValid { get; private set; }
        public T Result { get; private set; }
        public object Data { get; private set; }

        /// <inheritdoc cref="IRequestResult.Succeeded"/>
        public static RequestResult<T> Succeeded(T result)
        {
            return new RequestResult<T> { Success = true, Result = result };
        }

        /// <inheritdoc cref="IRequestResult.NotFound"/>
        public static RequestResult<T> NotFound(object data = null)
        {
            return new RequestResult<T> { IsNotFound = true, Data = data };
        }

        /// <inheritdoc cref="IRequestResult.NotValid"/>
        public static RequestResult<T> NotValid(object data = null)
        {
            return new RequestResult<T> { IsNotValid = true, Data = data };
        }

        static IRequestResult IRequestResult.NotFound(object data)
        {
            return NotFound(data);
        }

        static IRequestResult IRequestResult.NotValid(object data)
        {
            return NotValid(data);
        }

        static IRequestResult IRequestResult.Succeeded()
        {
            return Succeeded(default);
        }

        public IResult AsApiResult()
        {
            if (IsNotFound)
            {
                return Results.NotFound(Data);
            }

            if (IsNotValid)
            {
                return Results.BadRequest(Data);
            }

            return Results.Problem();
        }
    }

    /// <summary>
    /// Defines the members and operations implemented by <see cref="RequestResult{T}"/>.
    /// </summary>
    public interface IRequestResult
    {
        /// <summary>
        /// A flag indicating the request success status.
        /// </summary>
        bool Success { get; }

        /// <summary>
        /// A flag indicating the request result was not found.
        /// </summary>
        bool IsNotFound { get; }

        /// <summary>
        /// A flag indicating the request was invalid.
        /// </summary>
        bool IsNotValid { get; }

        /// <summary>
        /// Data object for holding any request related data other than the result.
        /// </summary>
        object Data { get; }

        /// <summary>
        /// Create a request result with success status.
        /// </summary>
        /// <returns>An instance of <see cref="IRequestResult"/>.</returns>
        public static abstract IRequestResult Succeeded();

        /// <summary>
        /// Create a request result with not found status.
        /// </summary>
        /// <param name="data">Data returned as part of the result.</param>
        /// <returns>An instance of <see cref="IRequestResult"/>.</returns>
        public static abstract IRequestResult NotFound(object data);

        /// <summary>
        /// Create a request result with invalid status.
        /// </summary>
        /// <param name="data">Data returned as part of the result.</param>
        /// <returns>An instance of <see cref="IRequestResult"/>.</returns>
        public static abstract IRequestResult NotValid(object data);
    }
}
