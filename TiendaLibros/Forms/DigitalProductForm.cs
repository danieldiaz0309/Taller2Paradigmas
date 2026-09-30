using System.ComponentModel;
using TiendaLibros.Models;
using TiendaLibros.Services;

namespace TiendaLibros.Forms
{
    [DesignerCategory("")]
    public class DigitalProductForm : ProductForm<DigitalProduct>
    {
        private readonly ComboBox _cmbFormat = new() { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly TextBox _txtUrl = new();

        public DigitalProductForm(ProductService service, DigitalProduct? existing = null)
            : base("libro digital", "MB", service, existing)
        {
            _cmbFormat.Items.AddRange(new object[] { "PDF", "EPUB", "MOBI", "MP3 (audiolibro)" });
            AddRow("Formato:", _cmbFormat);
            AddRow("URL de descarga:", _txtUrl);
            FinishSetup();
        }

        protected override string? ValidateSpecificFields()
        {
            if (_cmbFormat.SelectedIndex < 0)
                return "Seleccione un formato.";
            if (!Uri.TryCreate(_txtUrl.Text.Trim(), UriKind.Absolute, out _))
                return "La URL de descarga no es válida (ej: https://libros.com/descarga/123).";
            return null;
        }

        protected override void LoadSpecificFields(DigitalProduct product)
        {
            _cmbFormat.SelectedItem = product.Format;
            _txtUrl.Text = product.DownloadUrl;
        }

        protected override void ApplySpecificFields(DigitalProduct product)
        {
            product.Format = _cmbFormat.Text;
            product.DownloadUrl = _txtUrl.Text.Trim();
        }
    }
}