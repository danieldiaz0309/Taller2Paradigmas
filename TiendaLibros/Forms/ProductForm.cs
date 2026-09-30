using System.ComponentModel;
using TiendaLibros.Models;
using TiendaLibros.Services;

namespace TiendaLibros.Forms
{
    [DesignerCategory("")]
    public abstract class ProductForm<T> : Form where T : Product, new()
    {
        private readonly ProductService _service;
        private readonly T? _existing;
        private readonly TableLayoutPanel _layout;

        private readonly TextBox _txtCode = new();
        private readonly TextBox _txtName = new();
        private readonly TextBox _txtAuthor = new();
        private readonly TextBox _txtDescription = new();
        private readonly TextBox _txtCategory = new();
        private readonly NumericUpDown _numPrice = new() { Maximum = 100_000_000, ThousandsSeparator = true };
        private readonly NumericUpDown _numWeight = new() { Maximum = 100_000, DecimalPlaces = 2 };

        protected ProductForm(string title, string weightUnit, ProductService service, T? existing)
        {
            _service = service;
            _existing = existing;

            Text = existing == null ? $"Nuevo {title}" : $"Editar {title}";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(10);

            _layout = new TableLayoutPanel { ColumnCount = 2, AutoSize = true, Dock = DockStyle.Fill };
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 320));
            Controls.Add(_layout);

            AddRow("ISBN:", _txtCode);
            AddRow("Título:", _txtName);
            AddRow("Autor:", _txtAuthor);
            AddRow("Descripción:", _txtDescription);
            AddRow("Categoría:", _txtCategory);
            AddRow("Precio:", _numPrice);
            AddRow($"Peso ({weightUnit}):", _numWeight);
        }

        protected void AddRow(string label, Control control)
        {
            control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left });
            _layout.Controls.Add(control);
        }

        // Las hijas la llaman al final de su constructor, después de agregar sus campos
        protected void FinishSetup()
        {
            var btnSave = new Button { Text = "Guardar", AutoSize = true };
            var btnCancel = new Button { Text = "Cancelar", AutoSize = true, DialogResult = DialogResult.Cancel };
            btnSave.Click += (_, _) => Save();

            var buttons = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                AutoSize = true,
                Anchor = AnchorStyles.Right
            };
            buttons.Controls.Add(btnCancel);
            buttons.Controls.Add(btnSave);
            _layout.Controls.Add(buttons);
            _layout.SetColumnSpan(buttons, 2);

            AcceptButton = btnSave;
            CancelButton = btnCancel;

            if (_existing != null)
            {
                LoadCommonFields(_existing);
                LoadSpecificFields(_existing);
                _txtCode.ReadOnly = true; // el ISBN es la llave, no se edita
            }
        }

        private void Save()
        {
            if (string.IsNullOrWhiteSpace(_txtCode.Text) || string.IsNullOrWhiteSpace(_txtName.Text))
            {
                ShowWarning("El ISBN y el título son obligatorios.");
                return;
            }

            string? specificError = ValidateSpecificFields();
            if (specificError != null)
            {
                ShowWarning(specificError);
                return;
            }

            try
            {
                T product = _existing ?? new T();
                ApplyCommonFields(product);
                ApplySpecificFields(product);

                if (_existing == null) _service.Add(product);
                else _service.Update(product);

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadCommonFields(T product)
        {
            _txtCode.Text = product.Code;
            _txtName.Text = product.Name;
            _txtAuthor.Text = product.Author;
            _txtDescription.Text = product.Description;
            _txtCategory.Text = product.Category;
            _numPrice.Value = product.Price;
            _numWeight.Value = (decimal)product.Weight;
        }

        private void ApplyCommonFields(T product)
        {
            product.Code = _txtCode.Text.Trim();
            product.Name = _txtName.Text.Trim();
            product.Author = _txtAuthor.Text.Trim();
            product.Description = _txtDescription.Text.Trim();
            product.Category = _txtCategory.Text.Trim();
            product.Price = _numPrice.Value;
            product.Weight = (double)_numWeight.Value;
        }

        private static void ShowWarning(string message) =>
            MessageBox.Show(message, "Datos incompletos", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        protected virtual string? ValidateSpecificFields() => null;
        protected abstract void LoadSpecificFields(T product);
        protected abstract void ApplySpecificFields(T product);
    }
}