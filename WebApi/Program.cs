using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using WebApi.Helpers.Jwt;
using WebApi.DataAccess.Abstract;
using WebApi.DataAccess.Concrete;
using WebApi.Business.Concrete;
using WebApi.Business.Abstract;
using WebApi.DataAccess.Context;
using WebApi.DataAccess.Redis;
using Serilog;
using WebApi.Helpers.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using FluentValidation;
using WebApi.Helpers.Validator;
using FluentValidation.AspNetCore;
using WebApi.Entities.Concrete;
using Microsoft.AspNetCore.Authorization;
using WebApi.Helpers.Authorization;
using WebApi.Helpers.Swagger;
using WebApi.Helpers.Hashing;
using AutoMapper;
using WebApi.Entities.Concrete.Dtos;

// Serilog
var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
	.Filter.ByIncludingOnly(logEvent =>
	{
		var isRequestLog = logEvent.Properties.ContainsKey("IsRequestLog");
		var isError = logEvent.Level == Serilog.Events.LogEventLevel.Error;
		return isRequestLog || isError;
	})
	.WriteTo.MSSqlServer(
		connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
		sinkOptions: new MSSqlServerSinkOptions
		{
			TableName = "Logs",
			AutoCreateSqlTable = true
		},
		restrictedToMinimumLevel: LogEventLevel.Information
	)
	.Enrich.FromLogContext()
	.CreateLogger();

builder.Host.UseSerilog();

// DbContext
builder.Services.AddDbContext<DatabaseContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// HASHING SERVİSİ
builder.Services.AddScoped<PasswordHasher>();

// REPOSITORYLER
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IRepository<UserRole>, EfRepository<UserRole>>();
builder.Services.AddScoped<IRepository<WebApi.Entities.Concrete.Role>, EfRepository<WebApi.Entities.Concrete.Role>>();

// MANAGER'LAR
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IAuthManager, AuthManager>();

// DİĞER SERVİSLER
builder.Services.AddScoped<JwtTokenGenerator>();


// REDIS CONNECTION (Docker'daki Redis için: Host adı 'redis')
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
	var config = builder.Configuration.GetSection("Redis")["redis"] ?? "redis:6379";
	return ConnectionMultiplexer.Connect(config);
});
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

// (Opsiyonel) .NET'in IDistributedCache için de Redis register'ı
builder.Services.AddStackExchangeRedisCache(options =>
{
	options.Configuration = builder.Configuration.GetSection("Redis")["redis"];
	options.InstanceName = builder.Configuration.GetSection("Redis")["InstanceName"];
});

// JWT Authentication
builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		// Prod’da açık tut (http ile test ediyorsan dev’de false yapabilirsin)
		options.RequireHttpsMetadata = true;
		options.SaveToken = false;                 // token’ı server tarafında saklama
		options.IncludeErrorDetails = false;       // hata detaylarını sızdırma (dev’de true olabilir)

		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

			ValidateIssuer = true,
			ValidIssuer = builder.Configuration["Jwt:Issuer"],

			ValidateAudience = true,
			ValidAudience = builder.Configuration["Jwt:Audience"],

			ValidateLifetime = true,
			RequireExpirationTime = true,
			ClockSkew = TimeSpan.Zero,             // 5 dk toleransı kapat
		};

		// Güvenli event’ler (token’ı asla loglama)
		options.Events = new JwtBearerEvents
		{
			OnTokenValidated = ctx =>
			{
				var name = ctx.Principal?.Identity?.Name ?? "-";
				Log.Information("JWT doğrulandı (user={User})", name);
				return Task.CompletedTask;
			},
			OnAuthenticationFailed = ctx =>
			{
				Log.Warning("JWT doğrulama başarısız: {Error}", ctx.Exception.Message);
				return Task.CompletedTask;
			},
			// Token’ı loglama: güvenlik nedeniyle kapattık
			OnMessageReceived = ctx =>
			{
				// İstersen burada custom “Authorization: Bearer” dışı bir taşıma kuralı uygularsın.
				return Task.CompletedTask;
			}
		};
	});

// KAYNAK BAZLI YETKİLENDİRME (Resource-Based Authorization) AYARLARI
builder.Services.AddAuthorization(options =>
{
	// "CanManageSelf" adında yeni bir yetkilendirme politikası tanımla.
	options.AddPolicy("CanManageSelf", policy =>
	{
		policy.Requirements.Add(new OwnerAuthorizationRequirement());
	});
});

// Authorization Handler'ı Dependency Injection'a kaydet.
builder.Services.AddSingleton<IAuthorizationHandler, OwnerAuthorizationHandler>();

// Swagger
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "WebApi", Version = "v1" });

	// YENİ EKLENEN SATIR: Swagger filtresini ekle.
	c.OperationFilter<SwaggerAuthorizedOperationFilter>();

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Description = "Token'ı şu formatta girin: Bearer <token>",
		Name = "Authorization",
		In = ParameterLocation.Header,
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT"
	});
	c.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				}
			},
			new string[] {}
		}
	});
});
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllers()
	.AddFluentValidation(fv =>
	{
		// Validatörleri otomatik olarak bul ve kaydet
		fv.RegisterValidatorsFromAssemblyContaining<Program>();

		fv.DisableDataAnnotationsValidation = false; 
	});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
try
{
	using (var scope = app.Services.CreateScope())
	{
		var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
		db.Database.Migrate();
	}
}
catch (Exception ex)
{
	// Hata logla ama uygulamayı patlatmasın
	Console.WriteLine("Migration error: " + ex.Message);
}

// Middleware ve pipeline
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();
app.UseMiddleware<RequestResponseLoggingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
builder.WebHost.UseUrls("http://0.0.0.0:80");
app.Run();
