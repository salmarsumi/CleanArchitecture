using CA.MediatR;

namespace CA.Api
{
    public static class ResultExtensions
    {
        public static IResult AsApiResult<T>(this RequestResult<T> result)
        {
            if (result.IsNotFound)
            {
                return Results.NotFound(result.Data);
            }

            if (result.IsNotValid)
            {
                return Results.BadRequest(result.Data);
            }

            return Results.Problem();
        }
    }
}
