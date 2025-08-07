using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Principal;

namespace ProductService.Entities
{
	[Table("Products")]
	public class Product 
	{
		[Key]
		[Column("ProductID")]
		public int ProductId { get; set; }

		[Column("ProductName")]
		public string ProductName { get; set; }

		[Column("CategoryID")]
		public int? CategoryId { get; set; }

		[Column("UnitPrice")]
		public decimal? UnitPrice { get; set; }

		[Column("UnitsInStock")]
		public short? UnitsInStock { get; set; }

	}
}
