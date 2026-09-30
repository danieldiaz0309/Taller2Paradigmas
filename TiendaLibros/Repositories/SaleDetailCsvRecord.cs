namespace TiendaLibros.Repositories
{
    public class SaleDetailCsvRecord
    {
        public int SaleNumber { get; set; }       // enlace con sales.csv
        public string ProductCode { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal ShippingCost { get; set; }
    }
}
