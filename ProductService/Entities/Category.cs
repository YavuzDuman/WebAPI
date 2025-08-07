using System.ComponentModel.DataAnnotations.Schema;

namespace ProductService.Entities
{
	[Table("Categories")]
	public class Category
	{
		public int CategoryId { get; set; }
		public string CategoryName { get; set; }
	}
}
