using Microsoft.AspNetCore.SignalR.Client;

// Hub bağlantısını oluştur
// URL'yi kendi web api projenizin adresiyle değiştirin
var hubConnection = new HubConnectionBuilder()
	.WithUrl("https://localhost:7278/stock-hub") // Buraya kendi localhost adresinizi yazın
	.Build();

// Sunucudan gelen "ReceiveStockUpdate" sinyalini dinle
hubConnection.On<string>("ReceiveStockUpdate", (message) =>
{
	Console.WriteLine($"SignalR'dan mesaj geldi: {message}");
});

// Bağlantı kurulduğunda ne yapılacağını tanımla
hubConnection.Closed += async (error) =>
{
	Console.WriteLine("Bağlantı kesildi. Yeniden bağlanılıyor...");
	await Task.Delay(new Random().Next(0, 5) * 1000);
	await hubConnection.StartAsync();
};

try
{
	Console.WriteLine("Hub'a bağlanılıyor...");
	// Bağlantıyı başlat
	await hubConnection.StartAsync();
	Console.WriteLine("Bağlantı başarılı. Güncellemeler bekleniyor...");

	// Konsol uygulamasını açık tutmak için bir tuşa basılmasını bekle
	Console.WriteLine("Çıkmak için bir tuşa basın.");
	Console.ReadLine();

}
catch (Exception ex)
{
	Console.WriteLine($"Bağlantı kurulamadı: {ex.Message}");
}
