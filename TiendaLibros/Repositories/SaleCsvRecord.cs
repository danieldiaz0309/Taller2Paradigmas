namespace TiendaLibros.Repositories
{
    public class SaleCsvRecord
    {
        public int Number { get; set; }
        public DateTime Date { get; set; }
        public string CustomerDocument { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
    }
}