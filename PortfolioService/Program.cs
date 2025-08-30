using System.Data;
using Microsoft.Data.SqlClient;
using PortfolioService.DataAccess.Abstract;
using PortfolioService.DataAccess.Concrete;
using PortfolioService.Business.Abstract;
using PortfolioService.Business.Concrete;
using PortfolioService.DataAccess.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dapper i�in veritaban� ba�lant�s�n� ekle
builder.Services.AddScoped<IDbConnection>(sp =>
{
	var connectionString = builder.Configuration.GetConnectionString("PortfolioConnection");
	return new SqlConnection(connectionString);
});

// DbContext'i ba��ml�l�k enjeksiyonuna ekle
builder.Services.AddDbContext<PortfolioDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("PortfolioConnection")));

// HttpClient'� IStockApiService i�in ekle
builder.Services.AddHttpClient<IStockApiService, StockApiService>();

// Servislerin DI Container'a kayd�
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IPortfolioManager, PortfolioManager>();
builder.Services.AddScoped<IStockApiService, StockApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Veritaban� migration'lar�n� otomatik uygula
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<PortfolioDbContext>();
	db.Database.Migrate();
}

app.Run();
