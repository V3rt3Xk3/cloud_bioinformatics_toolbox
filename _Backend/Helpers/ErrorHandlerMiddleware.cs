using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Backend.Helpers
{
	public class ErrorHandlerMiddleware
	{
		private readonly RequestDelegate _next;

		public ErrorHandlerMiddleware(RequestDelegate next)
		{
			this._next = next;
		}

		public async Task Invoke(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception _error)
			{
				HttpResponse response = context.Response;
				response.ContentType = "application/json";

				int _statusCode = GetStatusCodeFromException(_error);
				response.StatusCode = _statusCode;

				string result = JsonSerializer.Serialize(new { message = _error?.Message });
				await response.WriteAsync(result);
			}
		}

		private static int GetStatusCodeFromException(Exception _error)
		{
			return _error switch
			{
				AppException e => (int)HttpStatusCode.BadRequest,
				KeyNotFoundException e => (int)HttpStatusCode.NotFound,
				_ => (int)HttpStatusCode.InternalServerError,
			};
		}
	}
}