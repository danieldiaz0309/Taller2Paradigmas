using System.ComponentModel;
using TiendaLibros.Models;
using TiendaLibros.Repositories;
using TiendaLibros.Services;

namespace TiendaLibros.Forms
{
    [DesignerCategory("")]
    public class MainForm : Form
    {
        private readonly ProductService _productService = new(new ProductRepository());
        private readonly CustomerService _customerService = new(new CsvRepository<Customer>("customers.csv"));
        private readonly SaleService _saleService;

        public MainForm()
        {
            _saleService = new SaleService(new SaleRepository(), _productService);
            _customerService.SetSalesChecker(_saleService.CustomerHasSales);

            Text = "Tienda de Libros";
            Size = new Size(380, 260);
            StartPosition = FormStartPosition.CenterScreen;

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                Padding = new Padding(20)
            };

            var btnProducts = new Button { Text = "Catálogo de libros", Width = 300, Height = 45 };
            btnProducts.Click += (_, _) =>
            {
                using var form = new CatalogForm(_productService);
                form.ShowDialog(this);
            };

            var btnCustomers = new Button { Text = "Clientes", Width = 300, Height = 45 };
            btnCustomers.Click += (_, _) =>
            {
                using var form = new CustomerListForm(_customerService);
                form.ShowDialog(this);
            };

            var btnSales = new Button { Text = "Ventas", Width = 300, Height = 45 };
            btnSales.Click += (_, _) =>
            {
                using var form = new SaleListForm(_saleService, _customerService, _productService);
                form.ShowDialog(this);
            };

            panel.Controls.AddRange(new Control[] { btnProducts, btnCustomers, btnSales });
            Controls.Add(panel);
        }
    }
}