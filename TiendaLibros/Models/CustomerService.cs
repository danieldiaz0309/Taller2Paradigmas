using System.Net.Mail;
using TiendaLibros.Models;
using TiendaLibros.Repositories;

namespace TiendaLibros.Services
{
    public class CustomerService
    {
        private const int MinPhoneLength = 7;

        private readonly CsvRepository<Customer> _repository;
        private readonly List<Customer> _customers;

        // Pregunta "¿este documento tiene compras?". Se conecta a las ventas en el paso 6.
        private Func<string, bool> _hasSales = _ => false;

        public CustomerService(CsvRepository<Customer> repository)
        {
            _repository = repository;
            _customers = repository.LoadAll();
        }

        public void SetSalesChecker(Func<string, bool> hasSales) => _hasSales = hasSales;

        public IReadOnlyList<Customer> GetAll() => _customers.AsReadOnly();

        public Customer? FindByDocument(string document) =>
            _customers.FirstOrDefault(c => c.Document.Equals(document.Trim(), StringComparison.OrdinalIgnoreCase));

        public void Add(Customer customer)
        {
            Normalize(customer);
            Validate(customer);
            if (FindByDocument(customer.Document) != null)
                throw new InvalidOperationException($"Ya existe un cliente con el documento {customer.Document}.");

            _customers.Add(customer);
            SaveChanges();
        }

        public void Update(string originalDocument, Customer updated)
        {
            var existing = FindByDocument(originalDocument)
                ?? throw new InvalidOperationException("El cliente no existe.");

            Normalize(updated);
            Validate(updated);

            bool documentChanged = !existing.Document.Equals(updated.Document, StringComparison.OrdinalIgnoreCase);
            if (documentChanged)
            {
                if (FindByDocument(updated.Document) != null)
                    throw new InvalidOperationException($"Ya existe un cliente con el documento {updated.Document}.");
                if (_hasSales(existing.Document))
                    throw new InvalidOperationException("No se puede cambiar el documento de un cliente que ya tiene compras.");
            }

            int index = _customers.IndexOf(existing);
            _customers[index] = updated;
            SaveChanges();
        }

        public void Delete(string document)
        {
            var customer = FindByDocument(document)
                ?? throw new InvalidOperationException("El cliente no existe.");

            if (_hasSales(customer.Document))
                throw new InvalidOperationException($"No se puede eliminar a {customer.Name}: ya tiene compras registradas.");

            _customers.Remove(customer);
            SaveChanges();
        }

        private void SaveChanges() => _repository.SaveAll(_customers);

        private static void Normalize(Customer customer)
        {
            customer.Document = customer.Document.Trim();
            customer.Name = customer.Name.Trim();
            customer.Email = customer.Email.Trim();
            customer.Phone = customer.Phone.Trim();
        }

        private static void Validate(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.Document) || string.IsNullOrWhiteSpace(customer.Name) ||
                string.IsNullOrWhiteSpace(customer.Email) || string.IsNullOrWhiteSpace(customer.Phone))
                throw new ArgumentException("Todos los campos son obligatorios.");

            if (!customer.Document.All(char.IsDigit))
                throw new ArgumentException("El documento solo debe tener números.");

            if (!MailAddress.TryCreate(customer.Email, out _))
                throw new ArgumentException("El correo no es válido.");

            if (!customer.Phone.All(char.IsDigit) || customer.Phone.Length < MinPhoneLength)
                throw new ArgumentException($"El teléfono debe tener solo números y al menos {MinPhoneLength} dígitos.");
        }
    }
}