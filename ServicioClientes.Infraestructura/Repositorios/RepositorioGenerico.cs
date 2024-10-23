using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ServicioClientes.Infraestructura.Contextos;

namespace ServicioClientes.Infraestructura.Repositorios
{
    public class RepositorioGenerico<T> : IRepositorioGenerico<T> where T : class
    {
        private readonly ServicioClientesContexto _contexto;
        private readonly DbSet<T> _dbSet;

        public RepositorioGenerico(ServicioClientesContexto contexto)
        {
            _contexto = contexto;
            _dbSet = _contexto.Set<T>();
        }


        public T ObtenerPorId(object id)
        {
            return _dbSet.Find(id);
        }


        public async Task<T> ObtenerPorIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }


        public IEnumerable<T> ObtenerTodos()
        {
            return _dbSet.ToList();
        }


        public async Task<IEnumerable<T>> ObtenerTodosAsync(Func<IQueryable<T>, IQueryable<T>> include = null)
        {
            IQueryable<T> query = _contexto.Set<T>();  

            
            if (include != null)
            {
                query = include(query);
            }

            return await query.ToListAsync(); 
        }



        public IEnumerable<T> Buscar(Expression<Func<T, bool>> predicado)
        {
            return _dbSet.Where(predicado).ToList();
        }


        public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado)
        {
            return await _dbSet.Where(predicado).ToListAsync();
        }


        public void Insertar(T entidad)
        {
            _dbSet.Add(entidad);
        }


        public async Task InsertarAsync(T entidad)
        {
            await _dbSet.AddAsync(entidad);
        }


        public void Actualizar(T entidad)
        {
            _dbSet.Attach(entidad);
            _contexto.Entry(entidad).State = EntityState.Modified;
        }


        public void Eliminar(T entidad)
        {
            if (_contexto.Entry(entidad).State == EntityState.Detached)
            {
                _dbSet.Attach(entidad);
            }
            _dbSet.Remove(entidad);
        }

        public void Guardar()
        {
            _contexto.SaveChanges();
        }

        public async Task ActualizarAsync(T entidad)
        {
            _dbSet.Attach(entidad);
            _contexto.Entry(entidad).State = EntityState.Modified;
            await Task.CompletedTask;
        }

        public async Task EliminarAsync(T entidad)
        {
            if (_contexto.Entry(entidad).State == EntityState.Detached)
            {
                _dbSet.Attach(entidad);
            }
            _dbSet.Remove(entidad);
            await Task.CompletedTask;
        }

        public async Task GuardarAsync()
        {
            await _contexto.SaveChangesAsync();
        }
    }
}

