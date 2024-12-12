using System.Net;
using System.Net.Mime;
using System.Text.Json;
using WZDemo.API.Exceptions;
using WZDemo.API.Models;

namespace WZDemo.API.Middlewares
{
	/// <summary>
	///	全域異常處理中間件
	/// </summary>
	public class ExceptionHandlerMiddleware
	{
		private readonly ILogger<ExceptionHandlerMiddleware> logger;
		private readonly RequestDelegate next;

		public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger,
			RequestDelegate next)
		{
			this.logger = logger;
			this.next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, ex.Message);
				await HandleCustomExceptionResponseAsync(context, ex);
			}
		}

		private async Task HandleCustomExceptionResponseAsync(HttpContext context, Exception ex)
		{
			context.Response.ContentType = MediaTypeNames.Application.Json;

			// 根據例外類型設置狀態碼
			if (ex is ResourceNotFoundException)
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			}
			else if (ex is KeyNotFoundException)
			{
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
			}
			else if (ex is ArgumentException)
			{
				context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
			}
			else if (ex is UnauthorizedAccessException)
			{
				context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
			}
			else
			{
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			}

			var response = new ErrorModel(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString());
			var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

			var json = JsonSerializer.Serialize(response, options);
			await context.Response.WriteAsync(json);
		}
	}
}
