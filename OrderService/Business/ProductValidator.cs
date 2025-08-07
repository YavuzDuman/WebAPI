namespace OrderService.Business
{
	public class ProductValidator
	{
		private readonly HttpClient _httpClient;

		public ProductValidator(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<bool> ProductExists(int productId)
		{
			var response = await _httpClient.GetAsync("http://productservice/api/Products/getbyid?id=" + productId);

			return response.IsSuccessStatusCode;
		}
	}
}
