using ImOdNotes.Core.Entities;

namespace ImOdNotes.Configuration
{
    public class PaginaConfig
    {
        private static readonly Dictionary<Type, int> RegistrosPorPagina = new()
        {
            {typeof(Evento), 10 },
            {typeof(Gasto), 10 },
            {typeof(Nota), 8 },
            {typeof(Tarea), 10 },
            {typeof(Usuario), 10 },
            {typeof(Rol), 10 },
            //{typeof() },
        };
        public static int Get<T>()
        {
            return RegistrosPorPagina.TryGetValue(typeof(T), out var Tamanno) ? Tamanno : 10;
        }
    }
}
