using TiendaLibros.Models;

namespace TiendaLibros.Repositories
{
    public class ProductRepository
    {
        private const string PhysicalType = "Physical";
        private const string DigitalType = "Digital";

        private readonly CsvRepository<ProductCsvRecord> _csv = new("products.csv");

        public List<Product> LoadAll() => _csv.LoadAll().Select(ToProduct).ToList();

        public void SaveAll(IEnumerable<Product> products) => _csv.SaveAll(products.Select(ToRecord));

        // Objeto → fila plana
        private static ProductCsvRecord ToRecord(Product product)
        {
            var record = new ProductCsvRecord
            {
                Code = product.Code,
                Name = product.Name,
                Author = product.Author,
                Description = product.Description,
                Category = product.Category,
                Price = product.Price,
                Weight = product.Weight
            };

            switch (product)
            {
                case PhysicalProduct physical:
                    record.Type = PhysicalType;
                    record.Stock = physical.Stock;
                    record.ShippingCost = physical.ShippingCost;
                    break;
                case DigitalProduct digital:
                    record.Type = DigitalType;
                    record.Format = digital.Format;
                    record.DownloadUrl = digital.DownloadUrl;
                    break;
                default:
                    throw new NotSupportedException($"Tipo de libro no soportado: {product.GetType().Name}");
            }
            return record;
        }

        // Fila plana → objeto del tipo correcto, según la columna Type
        private static Product ToProduct(ProductCsvRecord record)
        {
            Product product = record.Type switch
            {
                PhysicalType => new PhysicalProduct
                {
                    Stock = record.Stock ?? 0,
                    ShippingCost = record.ShippingCost ?? 0
                },
                DigitalType => new DigitalProduct
                {
                    Format = record.Format ?? string.Empty,
                    DownloadUrl = record.DownloadUrl ?? string.Empty
                },
                _ => throw new InvalidDataException($"Tipo de libro desconocido en el CSV: '{record.Type}'.")
            };

            product.Code = record.Code;
            product.Name = record.Name;
            product.Author = record.Author;
            product.Description = record.Description;
            product.Category = record.Category;
            product.Price = record.Price;
            product.Weight = record.Weight;
            return product;
        }
    }
}
