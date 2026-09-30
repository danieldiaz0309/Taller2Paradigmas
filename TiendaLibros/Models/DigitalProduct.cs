namespace TiendaLibros.Models
{
    public class DigitalProduct : Product
    {
        public string Format { get; set; } = string.Empty;      // PDF, EPUB, MOBI, MP3 (audiolibro)
        public string DownloadUrl { get; set; } = string.Empty;

        public override string TypeName => "Digital";
        public override string WeightDisplay => $"{Weight} MB";
        public override string Details => $"{Format} | {DownloadUrl}";

        public override decimal GetShippingCost(int quantity) => 0m;   // no se envía
        public override bool HasStock(int quantity) => quantity > 0;   // nunca se agota
        public override void ReduceStock(int quantity) { }             // nada que descontar
        public override void RestoreStock(int quantity) { }
    }
}