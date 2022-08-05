using Microsoft.AspNetCore.Http;

namespace CA.MediatR
{
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

        public static RequestResult<T> Succeeded(T result)
        {
            return new RequestResult<T> { Success = true, Result = result };
        }

        public static RequestResult<T> NotFound(object data = null)
        {
            return new RequestResult<T> { IsNotFound = true, Data = data };
        }

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

    public interface IRequestResult
    {
        bool Success { get; }
        bool IsNotFound { get; }
        bool IsNotValid { get; }
        object Data { get; }

        public static abstract IRequestResult Succeeded();
        public static abstract IRequestResult NotFound(object data);
        public static abstract IRequestResult NotValid(object data);
    }
}
