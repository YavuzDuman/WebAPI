using Microsoft.EntityFrameworkCore;
using StockService.Business.Abstract;
using StockService.Business.Concrete;
using StockService.DataAccess.Abstract;
using StockService.DataAccess.Concrete;
using StockService.DataAccess.Context;
using StockService.DataAccess.Redis;
using StackExchange.Redis; // StackExchange.Redis paketini kullanmak i�in
using Microsoft.Extensions.Hosting;
using StockService.BackgroundServices; // IHostedService i�in

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<StockDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DI Container'a servisleri ekle
builder.Services.AddHttpClient();

// Redis ba�lant�s�n� singleton olarak ekle
// Bu, uygulaman�n ya�am d�ng�s� boyunca tek bir Redis ba�lant�s�n�n kullan�lmas�n� sa�lar.
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
	var config = builder.Configuration.GetValue<string>("Redis:Configuration");
	return ConnectionMultiplexer.Connect(config);
});

// RedisCacheService'i de singleton olarak ekle
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();

// SignalR servisini ekle
builder.Services.AddSignalR();

// Di�er servislerin kayd�
builder.Services.AddScoped<IExternalApiService, ExternalApiService>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IStockManager, StockManager>();

// BackgroundService'i ekle
builder.Services.AddHostedService<StockUpdateWorker>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Routing'i ve Authorization'� etkinle�tir
app.UseRouting();
app.UseAuthorization();

// SignalR Hub'� haritaland�r
app.UseEndpoints(endpoints =>
{
	endpoints.MapControllers();
	endpoints.MapHub<StockService.Hubs.StockHub>("/stockHub");
});

// Veritaban� migrate i�lemi
try
{
	using (var scope = app.Services.CreateScope())
	{
		var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
		db.Database.Migrate();
	}
}
catch (Exception ex)
{
	Console.WriteLine("Migration error: " + ex.Message);
}

app.Run();
