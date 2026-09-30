namespace TiendaLibros.Models
{
    // Registro histórico: todo es de solo lectura una vez creada
    public class Sale
    {
        private readonly List<SaleDetail> _details;

        public int Number { get; }
        public DateTime Date { get; }
        public string CustomerDocument { get; }
        public string CustomerName { get; }       // copia del nombre del día de la compra
        public IReadOnlyList<SaleDetail> Details => _details.AsReadOnly();
        public decimal Total => _details.Sum(d => d.Subtotal);

        public Sale(int number, DateTime date, string customerDocument, string customerName, IEnumerable<SaleDetail> details)
        {
            if (string.IsNullOrWhiteSpace(customerDocument))
                throw new ArgumentException("La venta debe tener un cliente.");

            _details = details.ToList();
            if (_details.Count == 0)
                throw new InvalidOperationException("Una venta sin detalles no se puede guardar.");

            Number = number;
            Date = date;
            CustomerDocument = customerDocument;
            CustomerName = customerName;
        }
    }
}