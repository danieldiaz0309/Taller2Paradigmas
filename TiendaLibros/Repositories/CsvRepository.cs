using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace TiendaLibros.Repositories
{
    public class CsvRepository<T>
    {
        // InvariantCulture: los decimales siempre se guardan con punto (12.5),
        // sin importar si Windows está configurado en español (12,5)
        private static readonly CsvConfiguration Config = new(CultureInfo.InvariantCulture);

        private readonly string _filePath;

        public CsvRepository(string fileName)
        {
            Directory.CreateDirectory(DataPaths.Folder);
            _filePath = Path.Combine(DataPaths.Folder, fileName);
        }

        public List<T> LoadAll()
        {
            if (!File.Exists(_filePath))
                return new List<T>();

            using var reader = new StreamReader(_filePath);
            using var csv = new CsvReader(reader, Config);
            return csv.GetRecords<T>().ToList();
        }

        public void SaveAll(IEnumerable<T> records)
        {
            using var writer = new StreamWriter(_filePath);
            using var csv = new CsvWriter(writer, Config);
            csv.WriteRecords(records);
        }
    }
}