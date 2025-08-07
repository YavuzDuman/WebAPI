using Microsoft.EntityFrameworkCore;
using ProductService.Entities;

namespace ProductService.DataAccess
{
	public class ProductRepository : IProductRepository
	{
		private readonly DatabaseContext _context;

		public ProductRepository(DatabaseContext context)
		{
			_context = context;
		}

		public void Add(Product product)
		{
			_context.Products.Add(product);
			_context.SaveChanges();
		}

		public void Delete(int id)
		{
			var product = _context.Products.FirstOrDefault(p=>p.ProductId == id);
			if (product != null)
			{
				_context.Products.Remove(product);
				_context.SaveChanges();
			}
			else
			{
				throw new KeyNotFoundException($"Product with ID {id} not found.");
			}
		}

		public List<Product> GetAll()
		{
			var products = _context.Products.ToList();
			return products;
		}

		public Product GetById(int id)
		{
			var product = _context.Products.FirstOrDefault(p => p.ProductId == id);
			return product;
		}

		public void Update(int id, Product product)
		{
			
			var productToUpdate = _context.Products.FirstOrDefault(p => p.ProductId == id);
			if (product != null)
			{
				product.ProductName = product.ProductName;
				product.CategoryId = product.CategoryId;
				product.UnitsInStock = product.UnitsInStock;
				product.UnitPrice = product.UnitPrice;
				_context.Products.Update(product);
				_context.SaveChanges();
			}
			else
			{
				throw new KeyNotFoundException($"Product with ID {id} not found.");
			}
		}
	}
}
