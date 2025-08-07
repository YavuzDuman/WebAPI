using Serilog.Context;
using System.Text;

namespace WebApi.Helpers.Middleware
{
	public class RequestResponseLoggingMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<RequestResponseLoggingMiddleware> _logger;

		public RequestResponseLoggingMiddleware(
			RequestDelegate next, 
			ILogger<RequestResponseLoggingMiddleware> logger
			)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			context.Request.EnableBuffering();
			var buffer = new byte[Convert.ToInt32(context.Request.ContentLength ?? 0)];
			await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
			var requestBody = Encoding.UTF8.GetString(buffer);
			context.Request.Body.Position = 0;

			using (LogContext.PushProperty("IsRequestLog", true))
			{
				_logger.LogInformation("📥 Request: {Method} {Path} - Body: {Body}",
					context.Request.Method, context.Request.Path, requestBody);
			}
			var originalBodyStream = context.Response.Body;
			using var responseBody = new MemoryStream();
			context.Response.Body = responseBody;

			await _next(context);

			context.Response.Body.Seek(0, SeekOrigin.Begin);
			var responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
			context.Response.Body.Seek(0, SeekOrigin.Begin);

			using (LogContext.PushProperty("IsRequestLog", true))
			{
				_logger.LogInformation("📤 Response: {StatusCode} - Body: {Body}",
					context.Response.StatusCode, responseText);
			}
			await responseBody.CopyToAsync(originalBodyStream);
		}
	}

}
