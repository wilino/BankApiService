using System.Linq.Expressions;

namespace ServicioClientes.Infraestructura.Repositorios
{
    public interface IRepositorioGenerico<T> where T : class
    {      
        T ObtenerPorId(object id);
        IEnumerable<T> ObtenerTodos();
        IEnumerable<T> Buscar(Expression<Func<T, bool>> predicado);
        void Insertar(T entidad);
        void Actualizar(T entidad);
        void Eliminar(T entidad);
        void Guardar();

       
        Task<T> ObtenerPorIdAsync(object id);
        Task<IEnumerable<T>> ObtenerTodosAsync(Func<IQueryable<T>, IQueryable<T>> include);
        Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado);
        Task InsertarAsync(T entidad);
        Task GuardarAsync();
        Task ActualizarAsync(T entidad);
        Task EliminarAsync(T entidad);
    }
}

