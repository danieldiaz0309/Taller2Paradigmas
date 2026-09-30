using System.ComponentModel;
using TiendaLibros.Models;
using TiendaLibros.Services;

namespace TiendaLibros.Forms
{
    [DesignerCategory("")]
    public class CatalogForm : Form
    {
        private readonly ProductService _service;
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

        public CatalogForm(ProductService service)
        {
            _service = service;

            Text = "Catálogo de libros";
            Size = new Size(1100, 500);
            StartPosition = FormStartPosition.CenterParent;

            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(5) };
            buttons.Controls.Add(CreateButton("Nuevo libro físico", CreatePhysical));
            buttons.Controls.Add(CreateButton("Nuevo libro digital", CreateDigital));
            buttons.Controls.Add(CreateButton("Editar", EditSelected));
            buttons.Controls.Add(CreateButton("Eliminar", DeleteSelected));

            Controls.Add(_grid);    // primero la grilla (Fill)
            Controls.Add(buttons);  // después la barra (Top)

            ConfigureColumns();
            _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) EditSelected(); };
            LoadGrid();
        }

        private static Button CreateButton(string text, Action action)
        {
            var button = new Button { Text = text, AutoSize = true };
            button.Click += (_, _) => action();
            return button;
        }

        private void ConfigureColumns()
        {
            _grid.Columns.Add("Type", "Tipo");
            _grid.Columns.Add("Code", "ISBN");
            _grid.Columns.Add("Name", "Título");
            _grid.Columns.Add("Author", "Autor");
            _grid.Columns.Add("Category", "Categoría");
            _grid.Columns.Add("Price", "Precio");
            _grid.Columns.Add("Weight", "Peso");
            _grid.Columns.Add("Details", "Detalles");
        }

        private void LoadGrid()
        {
            _grid.Rows.Clear();
            foreach (var product in _service.GetAll())
            {
                int index = _grid.Rows.Add(
                    product.TypeName,
                    product.Code,
                    product.Name,
                    product.Author,
                    product.Category,
                    $"${product.Price:N0}",
                    product.WeightDisplay,
                    product.Details);
                _grid.Rows[index].Tag = product;
            }
        }

        private Product? SelectedProduct => _grid.CurrentRow?.Tag as Product;

        private void CreatePhysical() => ShowProductForm(new PhysicalProductForm(_service));

        private void CreateDigital() => ShowProductForm(new DigitalProductForm(_service));

        private void EditSelected()
        {
            var product = SelectedProduct;
            if (product == null)
            {
                MessageBox.Show("Seleccione un libro.", "Editar");
                return;
            }

            // Elegir qué pantalla mostrar es decisión de la interfaz, no del negocio
            Form form = product switch
            {
                PhysicalProduct physical => new PhysicalProductForm(_service, physical),
                DigitalProduct digital => new DigitalProductForm(_service, digital),
                _ => throw new NotSupportedException("Tipo de libro no soportado.")
            };
            ShowProductForm(form);
        }

        private void DeleteSelected()
        {
            var product = SelectedProduct;
            if (product == null)
            {
                MessageBox.Show("Seleccione un libro.", "Eliminar");
                return;
            }

            var answer = MessageBox.Show($"¿Eliminar '{product.Name}'?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (answer != DialogResult.Yes) return;

            try
            {
                _service.Delete(product.Code);
                LoadGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowProductForm(Form form)
        {
            using (form)
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    LoadGrid();
            }
        }
    }
}