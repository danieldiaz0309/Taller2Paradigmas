using TiendaLibros.Models;
using TiendaLibros.Repositories;

namespace TiendaLibros.Services
{
    public class ProductService
    {
        private readonly ProductRepository _repository;
        private readonly List<Product> _products;

        public ProductService(ProductRepository repository)
        {
            _repository = repository;
            _products = repository.LoadAll();
        }

        public IReadOnlyList<Product> GetAll() => _products.AsReadOnly();

        public Product? FindByCode(string code) =>
            _products.FirstOrDefault(p => p.Code.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase));

        public void Add(Product product)
        {
            Validate(product);
            if (FindByCode(product.Code) != null)
                throw new InvalidOperationException($"Ya existe un libro con el ISBN {product.Code}.");
            _products.Add(product);
            SaveChanges();
        }

        public void Update(Product product)
        {
            Validate(product);
            if (!_products.Contains(product))
                throw new InvalidOperationException("El libro no existe en el catálogo.");
            SaveChanges();
        }

        public void Delete(string code)
        {
            var product = FindByCode(code)
                ?? throw new InvalidOperationException($"No existe un libro con el ISBN {code}.");
            _products.Remove(product);
            SaveChanges();
        }

        // Público porque las ventas lo van a usar al descontar stock
        public void SaveChanges() => _repository.SaveAll(_products);

        private static void Validate(Product product)
        {
            if (string.IsNullOrWhiteSpace(product.Code))
                throw new ArgumentException("El ISBN es obligatorio.");
            if (string.IsNullOrWhiteSpace(product.Name))
                throw new ArgumentException("El título es obligatorio.");
        }
    }
}