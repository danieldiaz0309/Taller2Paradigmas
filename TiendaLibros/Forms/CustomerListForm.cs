using System.ComponentModel;
using TiendaLibros.Models;
using TiendaLibros.Services;

namespace TiendaLibros.Forms
{
    [DesignerCategory("")]
    public class CustomerListForm : Form
    {
        private readonly CustomerService _service;
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

        public CustomerListForm(CustomerService service)
        {
            _service = service;

            Text = "Clientes";
            Size = new Size(800, 450);
            StartPosition = FormStartPosition.CenterParent;

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(5) };
            buttons.Controls.Add(CreateButton("Nuevo cliente", CreateCustomer));
            buttons.Controls.Add(CreateButton("Editar", EditSelected));
            buttons.Controls.Add(CreateButton("Eliminar", DeleteSelected));

            Controls.Add(_grid);
            Controls.Add(buttons);

            _grid.Columns.Add("Document", "Documento");
            _grid.Columns.Add("Name", "Nombre");
            _grid.Columns.Add("Email", "Correo");
            _grid.Columns.Add("Phone", "Teléfono");
            _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) EditSelected(); };

            LoadGrid();
        }

        private static Button CreateButton(string text, Action action)
        {
            var button = new Button { Text = text, AutoSize = true };
            button.Click += (_, _) => action();
            return button;
        }

        private void LoadGrid()
        {
            _grid.Rows.Clear();
            foreach (var customer in _service.GetAll())
            {
                int index = _grid.Rows.Add(customer.Document, customer.Name, customer.Email, customer.Phone);
                _grid.Rows[index].Tag = customer;
            }
        }

        private Customer? SelectedCustomer => _grid.CurrentRow?.Tag as Customer;

        private void CreateCustomer() => ShowCustomerForm(new CustomerForm(_service));

        private void EditSelected()
        {
            var customer = SelectedCustomer;
            if (customer == null)
            {
                MessageBox.Show("Seleccione un cliente.", "Editar");
                return;
            }
            ShowCustomerForm(new CustomerForm(_service, customer));
        }

        private void DeleteSelected()
        {
            var customer = SelectedCustomer;
            if (customer == null)
            {
                MessageBox.Show("Seleccione un cliente.", "Eliminar");
                return;
            }

            var answer = MessageBox.Show($"¿Eliminar a {customer.Name}?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            try
            {
                _service.Delete(customer.Document);
                LoadGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo eliminar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ShowCustomerForm(Form form)
        {
            using (form)
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadGrid();
            }
        }
    }
}
