namespace TiendaLibros.Models
{
    // El "carrito" mientras se arma la venta en pantalla
    public class SaleDraft
    {
        private readonly List<DraftLine> _lines = new();

        public Customer? Customer { get; set; }
        public bool IsEmpty => _lines.Count == 0;
        public decimal Total => BuildDetails().Sum(d => d.Subtotal);

        public IEnumerable<(Product Product, int Quantity)> Lines =>
            _lines.Select(l => (l.Product, l.Quantity));

        public void AddItem(Product product, int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("La cantidad debe ser mayor que cero.");

            var line = _lines.FirstOrDefault(l => l.Product == product);
            int newQuantity = (line?.Quantity ?? 0) + quantity;

            // Polimorfismo: el físico revisa su stock, el digital siempre dice que sí
            if (!product.HasStock(newQuantity))
                throw new InvalidOperationException($"No hay stock suficiente de '{product.Name}'. {product.Details}");

            if (line == null) _lines.Add(new DraftLine(product, quantity));
            else line.Quantity = newQuantity;
        }

        public void RemoveItem(string productCode) =>
            _lines.RemoveAll(l => l.Product.Code == productCode);

        public List<SaleDetail> BuildDetails() =>
            _lines.Select(l => SaleDetail.FromProduct(l.Product, l.Quantity)).ToList();

        private sealed class DraftLine
        {
            public DraftLine(Product product, int quantity)
            {
                Product = product;
                Quantity = quantity;
            }

            public Product Product { get; }
            public int Quantity { get; set; }
        }
    }
}