using System.ComponentModel;
using TiendaLibros.Models;
using TiendaLibros.Services;

namespace TiendaLibros.Forms
{
    [DesignerCategory("")]
    public class CustomerForm : Form
    {
        private readonly CustomerService _service;
        private readonly Customer? _existing;
        private readonly TableLayoutPanel _layout;

        private readonly TextBox _txtDocument = new();
        private readonly TextBox _txtName = new();
        private readonly TextBox _txtEmail = new();
        private readonly TextBox _txtPhone = new();

        public CustomerForm(CustomerService service, Customer? existing = null)
        {
            _service = service;
            _existing = existing;

            Text = existing == null ? "Nuevo cliente" : "Editar cliente";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Padding = new Padding(10);

            _layout = new TableLayoutPanel { ColumnCount = 2, AutoSize = true, Dock = DockStyle.Fill };
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 300));
            Controls.Add(_layout);

            AddRow("Documento:", _txtDocument);
            AddRow("Nombre:", _txtName);
            AddRow("Correo:", _txtEmail);
            AddRow("Teléfono:", _txtPhone);

            var btnSave = new Button { Text = "Guardar", AutoSize = true };
            var btnCancel = new Button { Text = "Cancelar", AutoSize = true, DialogResult = DialogResult.Cancel };
            btnSave.Click += (_, _) => Save();

            var buttons = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, AutoSize = true, Anchor = AnchorStyles.Right };
            buttons.Controls.Add(btnCancel);
            buttons.Controls.Add(btnSave);
            _layout.Controls.Add(buttons);
            _layout.SetColumnSpan(buttons, 2);

            AcceptButton = btnSave;
            CancelButton = btnCancel;

            if (existing != null)
            {
                _txtDocument.Text = existing.Document;
                _txtName.Text = existing.Name;
                _txtEmail.Text = existing.Email;
                _txtPhone.Text = existing.Phone;
            }
        }

        private void AddRow(string label, Control control)
        {
            control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            _layout.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left });
            _layout.Controls.Add(control);
        }

        private void Save()
        {
            var customer = new Customer
            {
                Document = _txtDocument.Text,
                Name = _txtName.Text,
                Email = _txtEmail.Text,
                Phone = _txtPhone.Text
            };

            try
            {
                if (_existing == null) _service.Add(customer);
                else _service.Update(_existing.Document, customer);

                DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "No se pudo guardar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}