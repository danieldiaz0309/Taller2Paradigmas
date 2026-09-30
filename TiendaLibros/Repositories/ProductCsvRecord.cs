namespace TiendaLibros.Repositories
{
    // Forma plana de un libro en el CSV. Los campos con ? quedan vacíos
    // cuando no aplican al tipo (ej: un digital no tiene Stock).
    public class ProductCsvRecord
    {
        public string Type { get; set; } = string.Empty;   // "Physical" o "Digital"
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double Weight { get; set; }

        // Solo físico
        public int? Stock { get; set; }
        public decimal? ShippingCost { get; set; }

        // Solo digital
        public string? Format { get; set; }
        public string? DownloadUrl { get; set; }
    }
}
