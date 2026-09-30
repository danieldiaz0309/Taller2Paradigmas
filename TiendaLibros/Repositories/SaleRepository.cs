using TiendaLibros.Models;

namespace TiendaLibros.Repositories
{
    public class SaleRepository
    {
        private readonly CsvRepository<SaleCsvRecord> _salesCsv = new("sales.csv");
        private readonly CsvRepository<SaleDetailCsvRecord> _detailsCsv = new("sale_details.csv");

        public List<Sale> LoadAll()
        {
            // Agrupa las filas sueltas de detalles por número de venta
            var detailsBySale = _detailsCsv.LoadAll().ToLookup(d => d.SaleNumber);

            return _salesCsv.LoadAll()
                .Select(r => new Sale(
                    r.Number,
                    r.Date,
                    r.CustomerDocument,
                    r.CustomerName,
                    detailsBySale[r.Number].Select(ToDetail)))
                .ToList();
        }

        public void SaveAll(IEnumerable<Sale> sales)
        {
            var list = sales.ToList();

            _salesCsv.SaveAll(list.Select(s => new SaleCsvRecord
            {
                Number = s.Number,
                Date = s.Date,
                CustomerDocument = s.CustomerDocument,
                CustomerName = s.CustomerName
            }));

            _detailsCsv.SaveAll(list.SelectMany(s => s.Details.Select(d => new SaleDetailCsvRecord
            {
                SaleNumber = s.Number,
                ProductCode = d.ProductCode,
                ProductDescription = d.ProductDescription,
                UnitPrice = d.UnitPrice,
                Quantity = d.Quantity,
                ShippingCost = d.ShippingCost
            })));
        }

        private static SaleDetail ToDetail(SaleDetailCsvRecord r) =>
            new(r.ProductCode, r.ProductDescription, r.UnitPrice, r.Quantity, r.ShippingCost);
    }
}
