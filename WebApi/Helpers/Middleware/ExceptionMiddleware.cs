using System.Net;
using System.Text.Json;
using WebApi.Helpers.ErrorHandling;

namespace WebApi.Helpers.Middleware
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;
		private readonly IHostEnvironment _env;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
		{
			_next = next;
			_logger = logger;
			_env = env;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "❌ Global hata yakalandı: {Message}", ex.Message);

				context.Response.ContentType = "application/json";
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

				var errorDetails = new ErrorDetails
				{
					StatusCode = context.Response.StatusCode,
					// Geliştirme ortamında daha detaylı hata mesajı göster
					Message = _env.IsDevelopment() ? ex.Message : "Sunucu tarafında beklenmedik bir hata oluştu."
				};

				await context.Response.WriteAsync(JsonSerializer.Serialize(errorDetails));
			}
		}
	}

}
