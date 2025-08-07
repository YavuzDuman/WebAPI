using System.Text.Json;

namespace WebApi.Helpers.Middleware
{
	public class ExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<ExceptionMiddleware> _logger;

		public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "❌ Global hata yakalandı: {Message} | StackTrace: {StackTrace}", ex.Message, ex.StackTrace);//hangi kod satırı hatalı

				context.Response.ContentType = "application/json";
				context.Response.StatusCode = 500;

				await context.Response.WriteAsync(JsonSerializer.Serialize(new
				{
					status = 500,
					message = "Sunucu hatası oluştu. Lütfen tekrar deneyin."
				}));
			}
		}
	}

}
