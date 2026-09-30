namespace TiendaLibros.Models
{
    // Copia de lo vendido tal como estaba ese día. No apunta al Product.
    public class SaleDetail
    {
        public string ProductCode { get; }          // solo como referencia (para anular)
        public string ProductDescription { get; }
        public decimal UnitPrice { get; }
        public int Quantity { get; }
        public decimal ShippingCost { get; }        // envío total de esta línea
        public decimal Subtotal => UnitPrice * Quantity + ShippingCost;

        public SaleDetail(string productCode, string productDescription, decimal unitPrice, int quantity, decimal shippingCost)
        {
            if (quantity <= 0) throw new ArgumentException("La cantidad debe ser mayor que cero.");
            if (unitPrice < 0) throw new ArgumentException("El precio no puede ser negativo.");
            if (shippingCost < 0) throw new ArgumentException("El envío no puede ser negativo.");

            ProductCode = productCode;
            ProductDescription = productDescription;
            UnitPrice = unitPrice;
            Quantity = quantity;
            ShippingCost = shippingCost;
        }

        // Toma la "foto" del libro en este momento
        public static SaleDetail FromProduct(Product product, int quantity) =>
            new(product.Code,
                $"{product.Name} - {product.Author} ({product.TypeName})",
                product.Price,
                quantity,
                product.GetShippingCost(quantity));
    }
}