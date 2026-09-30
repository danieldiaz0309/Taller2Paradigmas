namespace TiendaLibros.Models
{
    public class PhysicalProduct : Product
    {
        private int _stock;
        private decimal _shippingCost;

        public int Stock
        {
            get => _stock;
            set
            {
                if (value < 0) throw new ArgumentException("El stock no puede ser negativo.");
                _stock = value;
            }
        }

        // Costo de envío por unidad
        public decimal ShippingCost
        {
            get => _shippingCost;
            set
            {
                if (value < 0) throw new ArgumentException("El costo de envío no puede ser negativo.");
                _shippingCost = value;
            }
        }

        public override string TypeName => "Físico";
        public override string WeightDisplay => $"{Weight} kg";
        public override string Details => $"Stock: {Stock} | Envío: ${ShippingCost:N0} c/u";

        public override decimal GetShippingCost(int quantity) => ShippingCost * quantity;

        public override bool HasStock(int quantity) => quantity > 0 && Stock >= quantity;

        public override void ReduceStock(int quantity)
        {
            if (!HasStock(quantity))
                throw new InvalidOperationException($"Stock insuficiente de '{Name}'. Disponible: {Stock}.");
            Stock -= quantity;
        }

        public override void RestoreStock(int quantity) => Stock += quantity;
    }
}