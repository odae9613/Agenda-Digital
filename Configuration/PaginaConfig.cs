using ImOdNotes.Core.Entities;

namespace ImOdNotes.Configuration
{
    public class PaginaConfig
    {
        private static readonly Dictionary<Type, int> RegistrosPorPagina = new()
        {
            {typeof(Evento), 10 },
            {typeof(Gasto), 5 },
            {typeof(Nota), 8 },
            {typeof(Tarea), 10 },
            {typeof(Usuario), 3 },
            //{typeof() },
        };
        public static int Get<T>()
        {
            return RegistrosPorPagina.TryGetValue(typeof(T), out var Tamanno) ? Tamanno : 10;
        }
    }
}
