namespace WebApi.DataAccess.Abstract
{
	public interface IRedisCacheService
	{
		Task<string> GetValueAsync(string key);
		Task<bool> SetValueAsync(string key, string value);
		Task Clear(string key);
		void ClearAll();
	}
}
