using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Helpers.Swagger
{
	public class SwaggerAuthorizedOperationFilter : IOperationFilter
	{
		public void Apply(OpenApiOperation operation, OperationFilterContext context)
		{
			// Endpoint'te veya kontrolcüde [Authorize] özniteliği var mı kontrol et.
			var hasAuthorize =
				context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true ||
				context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>().Any() == true;

			if (!hasAuthorize)
			{
				// Yetkilendirme gerektirmeyen endpoint'ler için bir şey yapma.
				return;
			}

			// Endpoint yetkilendirme gerektiriyorsa, güvenlik tanımını ekle.
			operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
			operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

			var openApiSecurityScheme = new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			};

			operation.Security = new List<OpenApiSecurityRequirement>
			{
				new OpenApiSecurityRequirement
				{
					[openApiSecurityScheme] = new List<string>()
				}
			};
		}
	}
}
