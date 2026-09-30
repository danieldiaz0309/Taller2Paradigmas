namespace TiendaLibros.Models
{
    public abstract class Product
    {
        private decimal _price;
        private double _weight;

        public string Code { get; set; } = string.Empty;        // ISBN
        public string Name { get; set; } = string.Empty;        // Título
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;    // Novela, Ciencia ficción, Historia...

        public decimal Price
        {
            get => _price;
            set
            {
                if (value < 0) throw new ArgumentException("El precio no puede ser negativo.");
                _price = value;
            }
        }

        // Físico: kg. Digital: MB de descarga.
        public double Weight
        {
            get => _weight;
            set
            {
                if (value < 0) throw new ArgumentException("El peso no puede ser negativo.");
                _weight = value;
            }
        }

        // Comportamiento que cada tipo resuelve a su manera
        public abstract string TypeName { get; }
        public abstract string WeightDisplay { get; }
        public abstract string Details { get; }
        public abstract decimal GetShippingCost(int quantity);
        public abstract bool HasStock(int quantity);
        public abstract void ReduceStock(int quantity);
        public abstract void RestoreStock(int quantity);
    }
}
