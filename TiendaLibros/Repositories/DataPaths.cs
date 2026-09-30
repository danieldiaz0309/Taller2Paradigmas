namespace TiendaLibros.Repositories
{
    public static class DataPaths
    {
        // El .exe corre en bin\Debug\net8.0-windows. Subimos 3 niveles hasta la
        // carpeta del proyecto, para que los CSV queden dentro del proyecto
        // y se incluyan al comprimirlo para la entrega.
        public static readonly string Folder =
            Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Data"));
    }
}