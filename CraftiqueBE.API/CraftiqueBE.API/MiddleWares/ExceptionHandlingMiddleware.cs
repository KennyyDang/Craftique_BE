using System.Net;
using System.Text.Json;

namespace CraftiqueBE.API.MiddleWares
{
	public class ExceptionHandlingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IHostEnvironment _env;

		public ExceptionHandlingMiddleware(RequestDelegate next, IHostEnvironment env)
		{
			_next = next;
			_env = env;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				await HandleExceptionAsync(context, ex);
			}
		}

		private Task HandleExceptionAsync(HttpContext context, Exception ex)
		{
			var statusCode = ex switch
			{
				KeyNotFoundException => (int)HttpStatusCode.NotFound,          // 404 Not Found
				ArgumentException => (int)HttpStatusCode.BadRequest,           // 400 Bad Request
				InvalidOperationException => (int)HttpStatusCode.Conflict,     // 409 Conflict
				UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized, // 401 Unauthorized
				_ => (int)HttpStatusCode.InternalServerError                  // 500 Internal Server Error
			};

			var response = new
			{
				statusCode,
				message = ex.Message,
				errorType = ex.GetType().Name,
				stackTrace = _env.IsDevelopment() ? ex.StackTrace : null  // just show StackTrace in Development env
			};

			context.Response.ContentType = "application/json";
			context.Response.StatusCode = statusCode;
			return context.Response.WriteAsync(JsonSerializer.Serialize(response));
		}
	}
}
