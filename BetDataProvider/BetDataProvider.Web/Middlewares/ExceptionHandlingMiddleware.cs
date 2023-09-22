using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BetDataProvider.Web.Middlewares
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{

				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

				ProblemDetails problem = new()
				{
					Status = context.Response.StatusCode,
					Title = ex.Message
				};

				await context.Response.WriteAsJsonAsync(problem);
			}
        }
    }
}
