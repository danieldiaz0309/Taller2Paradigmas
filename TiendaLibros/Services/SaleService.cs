using TiendaLibros.Models;
using TiendaLibros.Repositories;

namespace TiendaLibros.Services
{
    public class SaleService
    {
        private readonly SaleRepository _repository;
        private readonly ProductService _productService;
        private readonly List<Sale> _sales;

        public SaleService(SaleRepository repository, ProductService productService)
        {
            _repository = repository;
            _productService = productService;
            _sales = repository.LoadAll();
        }

        public IReadOnlyList<Sale> GetAll() => _sales.AsReadOnly();

        public Sale? FindByNumber(int number) => _sales.FirstOrDefault(s => s.Number == number);

        public bool CustomerHasSales(string document) =>
            _sales.Any(s => s.CustomerDocument.Equals(document, StringComparison.OrdinalIgnoreCase));

        public Sale Register(SaleDraft draft)
        {
            if (draft.Customer == null)
                throw new InvalidOperationException("Seleccione un cliente.");
            if (draft.IsEmpty)
                throw new InvalidOperationException("Una venta sin detalles no se puede guardar.");

            // 1. Verificar todo antes de tocar nada
            foreach (var (product, quantity) in draft.Lines)
                if (!product.HasStock(quantity))
                    throw new InvalidOperationException($"No hay stock suficiente de '{product.Name}'.");

            // 2. Tomar la "foto" de los detalles
            var details = draft.BuildDetails();

            // 3. Descontar stock (el digital no hace nada)
            foreach (var (product, quantity) in draft.Lines)
                product.ReduceStock(quantity);

            var sale = new Sale(NextNumber(), DateTime.Now, draft.Customer.Document, draft.Customer.Name, details);
            _sales.Add(sale);

            _repository.SaveAll(_sales);
            _productService.SaveChanges();
            return sale;
        }

        // Anular: elimina la venta y devuelve el stock de los libros que aún existen
        public void Cancel(int number)
        {
            var sale = FindByNumber(number)
                ?? throw new InvalidOperationException($"No existe la venta #{number}.");

            foreach (var detail in sale.Details)
                _productService.FindByCode(detail.ProductCode)?.RestoreStock(detail.Quantity);

            _sales.Remove(sale);
            _repository.SaveAll(_sales);
            _productService.SaveChanges();
        }

        private int NextNumber() => _sales.Count == 0 ? 1 : _sales.Max(s => s.Number) + 1;
    }
}