using System.ComponentModel;
using TiendaLibros.Models;
using TiendaLibros.Services;

namespace TiendaLibros.Forms
{
    [DesignerCategory("")]
    public class SaleForm : Form
    {
        private readonly SaleService _saleService;
        private readonly SaleDraft _draft = new();

        private readonly ComboBox _cmbCustomer = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 350 };
        private readonly ComboBox _cmbProduct = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 400 };
        private readonly NumericUpDown _numQuantity = new() { Minimum = 1, Maximum = 1000, Value = 1, Width = 70 };
        private readonly Label _lblTotal = new() { AutoSize = true, Font = new Font("Segoe UI", 14, FontStyle.Bold) };
        private readonly DataGridView _grid = new()
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

        public SaleForm(SaleService saleService, CustomerService customerService, ProductService productService)
        {
            _saleService = saleService;

            Text = "Nueva venta";
            Size = new Size(1000, 550);
            StartPosition = FormStartPosition.CenterParent;

            _cmbCustomer.Format += (_, e) => { if (e.ListItem is Customer c) e.Value = $"{c.Name} - {c.Document}"; };
            _cmbProduct.Format += (_, e) => { if (e.ListItem is Product p) e.Value = $"{p.Name} ({p.TypeName}) - ${p.Price:N0}"; };
            _cmbCustomer.Items.AddRange(customerService.GetAll().Cast<object>().ToArray());
            _cmbProduct.Items.AddRange(productService.GetAll().Cast<object>().ToArray());

            var customerRow = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(5) };
            customerRow.Controls.Add(CreateLabel("Cliente:"));
            customerRow.Controls.Add(_cmbCustomer);

            var productRow = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(5) };
            productRow.Controls.Add(CreateLabel("Libro:"));
            productRow.Controls.Add(_cmbProduct);
            productRow.Controls.Add(CreateLabel("Cantidad:"));
            productRow.Controls.Add(_numQuantity);
            productRow.Controls.Add(CreateButton("Agregar", AddItem));
            productRow.Controls.Add(CreateButton("Quitar seleccionado", RemoveSelected));

            var bottomRow = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                AutoSize = true,
                FlowDirection = FlowDirection.RightToLeft,
                Padding = new Padding(5)
            };
            bottomRow.Controls.Add(CreateButton("Cancelar", () => DialogResult = DialogResult.Cancel));
            bottomRow.Controls.Add(CreateButton("Guardar venta", SaveSale));
            bottomRow.Controls.Add(_lblTotal);

            // Orden importa para el acoplamiento (Dock)
            Controls.Add(_grid);
            Controls.Add(bottomRow);
            Controls.Add(productRow);
            Controls.Add(customerRow);

            _grid.Columns.Add("Description", "Libro");
            _grid.Columns.Add("UnitPrice", "Precio unit.");
            _grid.Columns.Add("Quantity", "Cantidad");
            _grid.Columns.Add("Shipping", "Envío");
            _grid.Columns.Add("Subtotal", "Subtotal");

            RefreshDetails();
        }

        private static Label CreateLabel(string text) =>
            new() { Text = text, AutoSize = true, Margin = new Padding(3, 8, 3, 3) };

        private static Button CreateButton(string text, Action action)
        {
            var button = new Button { Text = text, AutoSize = true };
            button.Click += (_, _) => action();
            return button;
        }

        private void AddItem()
        {
            if (_cmbProduct.SelectedItem is not Product product)
            {
                MessageBox.Show("Seleccione un libro.", "Agregar");
                return;
            }

            try
            {
                _draft.AddItem(product, (int)_numQuantity.Value);
                _numQuantity.Value = 1;
                RefreshDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo agregar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void RemoveSelected()
        {
            if (_grid.CurrentRow?.Tag is string productCode)
            {
                _draft.RemoveItem(productCode);
                RefreshDetails();
            }
        }

        private void RefreshDetails()
        {
            _grid.Rows.Clear();
            foreach (var detail in _draft.BuildDetails())
            {
                int index = _grid.Rows.Add(
                    detail.ProductDescription,
                    $"${detail.UnitPrice:N0}",
                    detail.Quantity,
                    $"${detail.ShippingCost:N0}",
                    $"${detail.Subtotal:N0}");
                _grid.Rows[index].Tag = detail.ProductCode;
            }
            _lblTotal.Text = $"Total: ${_draft.Total:N0}";
        }

        private void SaveSale()
        {
            _draft.Customer = _cmbCustomer.SelectedItem as Customer;

            try
            {
                var sale = _saleService.Register(_draft);
                MessageBox.Show($"Venta #{sale.Number} registrada por ${sale.Total:N0}.", "Venta guardada");
                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}