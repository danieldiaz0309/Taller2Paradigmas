using System.ComponentModel;
using TiendaLibros.Models;
using TiendaLibros.Services;

namespace TiendaLibros.Forms
{
    [DesignerCategory("")]
    public class SaleListForm : Form
    {
        private readonly SaleService _saleService;
        private readonly CustomerService _customerService;
        private readonly ProductService _productService;

        private readonly DataGridView _salesGrid = CreateGrid();
        private readonly DataGridView _detailsGrid = CreateGrid();

        public SaleListForm(SaleService saleService, CustomerService customerService, ProductService productService)
        {
            _saleService = saleService;
            _customerService = customerService;
            _productService = productService;

            Text = "Ventas";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterParent;

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(5) };
            buttons.Controls.Add(CreateButton("Nueva venta", NewSale));
            buttons.Controls.Add(CreateButton("Anular venta", CancelSelected));

            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal };
            split.Panel1.Controls.Add(_salesGrid);
            split.Panel2.Controls.Add(_detailsGrid);
            split.Panel2.Controls.Add(new Label
            {
                Text = "Detalle de la venta seleccionada",
                Dock = DockStyle.Top,
                Height = 25,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
            Load += (_, _) => split.SplitterDistance = split.Height / 2;

            Controls.Add(split);
            Controls.Add(buttons);

            _salesGrid.Columns.Add("Number", "Número");
            _salesGrid.Columns.Add("Date", "Fecha");
            _salesGrid.Columns.Add("Customer", "Cliente");
            _salesGrid.Columns.Add("Document", "Documento");
            _salesGrid.Columns.Add("Total", "Total");

            _detailsGrid.Columns.Add("Description", "Libro");
            _detailsGrid.Columns.Add("UnitPrice", "Precio unit.");
            _detailsGrid.Columns.Add("Quantity", "Cantidad");
            _detailsGrid.Columns.Add("Shipping", "Envío");
            _detailsGrid.Columns.Add("Subtotal", "Subtotal");

            _salesGrid.SelectionChanged += (_, _) => ShowSelectedDetails();
            LoadSales();
        }

        private static DataGridView CreateGrid() => new()
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        private static Button CreateButton(string text, Action action)
        {
            var button = new Button { Text = text, AutoSize = true };
            button.Click += (_, _) => action();
            return button;
        }

        private void LoadSales()
        {
            _salesGrid.Rows.Clear();
            foreach (var sale in _saleService.GetAll())
            {
                int index = _salesGrid.Rows.Add(
                    sale.Number,
                    sale.Date.ToString("yyyy-MM-dd HH:mm"),
                    sale.CustomerName,
                    sale.CustomerDocument,
                    $"${sale.Total:N0}");
                _salesGrid.Rows[index].Tag = sale;
            }
            ShowSelectedDetails();
        }

        private void ShowSelectedDetails()
        {
            _detailsGrid.Rows.Clear();
            if (_salesGrid.CurrentRow?.Tag is not Sale sale) return;

            foreach (var detail in sale.Details)
            {
                _detailsGrid.Rows.Add(
                    detail.ProductDescription,
                    $"${detail.UnitPrice:N0}",
                    detail.Quantity,
                    $"${detail.ShippingCost:N0}",
                    $"${detail.Subtotal:N0}");
            }
        }

        private void NewSale()
        {
            using var form = new SaleForm(_saleService, _customerService, _productService);
            if (form.ShowDialog(this) == DialogResult.OK)
                LoadSales();
        }

        private void CancelSelected()
        {
            if (_salesGrid.CurrentRow?.Tag is not Sale sale)
            {
                MessageBox.Show("Seleccione una venta.", "Anular");
                return;
            }

            var answer = MessageBox.Show(
                $"¿Anular la venta #{sale.Number}? Se devolverá el stock de los libros físicos.",
                "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            try
            {
                _saleService.Cancel(sale.Number);
                LoadSales();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
