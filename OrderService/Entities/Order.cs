namespace OrderService.Entities
{
	public class Order
	{
		public int OrderId { get; set; }
		public int ProductId { get; set; }
		public int UserId { get; set; }
		public DateTime OrderDate { get; set; }
	}
}
