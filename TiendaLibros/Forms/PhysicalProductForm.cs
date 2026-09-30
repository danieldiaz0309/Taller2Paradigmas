using System.ComponentModel;
using TiendaLibros.Models;
using TiendaLibros.Services;

namespace TiendaLibros.Forms
{
    [DesignerCategory("")]
    public class PhysicalProductForm : ProductForm<PhysicalProduct>
    {
        private readonly NumericUpDown _numStock = new() { Maximum = 100_000 };
        private readonly NumericUpDown _numShipping = new() { Maximum = 1_000_000, ThousandsSeparator = true };

        public PhysicalProductForm(ProductService service, PhysicalProduct? existing = null)
            : base("libro físico", "kg", service, existing)
        {
            AddRow("Stock:", _numStock);
            AddRow("Envío por unidad:", _numShipping);
            FinishSetup();
        }

        protected override void LoadSpecificFields(PhysicalProduct product)
        {
            _numStock.Value = product.Stock;
            _numShipping.Value = product.ShippingCost;
        }

        protected override void ApplySpecificFields(PhysicalProduct product)
        {
            product.Stock = (int)_numStock.Value;
            product.ShippingCost = _numShipping.Value;
        }
    }
}
