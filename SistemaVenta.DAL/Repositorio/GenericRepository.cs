using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SistemaVenta.DAL.Repositorio.Contrato;
using SistemaVenta.DAL.DBContext;

namespace SistemaVenta.DAL.Repositorio
{
        public class GenericRepository<TModelo> : IGenericRepository<TModelo> where TModelo : class
        {
            private readonly DbContext _dbContext;

            public GenericRepository(DbContext dbContext)
            {
                _dbContext = dbContext;
            }
            public async Task<TModelo> Obtener(Expression<Func<TModelo, bool>> filtro)
            {
                try
                {
                    TModelo modelo = await _dbContext.Set<TModelo>().FirstOrDefaultAsync(filtro);
                    return modelo;
                }
                catch
                {
                    throw;
                }
            }
            public async Task<TModelo> Crear(TModelo modelo)
            {
                try
                {
                    _dbContext.Set<TModelo>().Add(modelo);
                    await _dbContext.SaveChangesAsync();
                    return modelo;
                }
                catch
                {
                    throw;
                }
            }
            public async Task<bool> Editar(TModelo modelo)
            {
                try
                {
                    _dbContext.Set<TModelo>().Update(modelo);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            public async Task<bool> Eliminar(TModelo modelo)
            {
                try
                {
                    _dbContext.Set<TModelo>().Remove(modelo);
                    await _dbContext.SaveChangesAsync();
                    return true;
                }
                catch
                {
                    throw;
                }
            }
            public async Task<IQueryable<TModelo>> Consultar(Expression<Func<TModelo, bool>> filtro = null)
            {
                try
                {
                    IQueryable<TModelo> Querymodelos = filtro == null ? _dbContext.Set<TModelo>() : _dbContext.Set<TModelo>().Where(filtro);
                    return Querymodelos;
                }
                catch
                {
                    throw;
                }
            }
        }
}
