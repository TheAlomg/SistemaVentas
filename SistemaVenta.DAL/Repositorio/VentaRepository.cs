using SistemaVenta.DAL.DBContext;
using SistemaVenta.DAL.Repositorio.Contrato;
using SistemaVenta.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SistemaVenta.DAL.Repositorio
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly BdventaContext _dbContext;

        public VentaRepository(BdventaContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Venta> Registrar(Venta modelo)
        {
            Venta ventaGenerada = new Venta();


            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetallesVenta dv in modelo.DetallesVenta)
                    {
                        Producto producto_Encontrado = _dbContext.Productos.Where(p => p.IdProducto == dv.IdProducto).First();
                        producto_Encontrado.Stock = producto_Encontrado.Stock - dv.Cantidad;

                        _dbContext.Productos.Update(producto_Encontrado);
                    }
                    await _dbContext.SaveChangesAsync();

                    NumeroDocumento correlativo = _dbContext.NumeroDocumentos.First();
                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaRegistro = DateTime.Now;
                    _dbContext.NumeroDocumentos.Update(correlativo);
                    await _dbContext.SaveChangesAsync();


                    int cantidad_Digitos = 4;
                    string ceros = string.Concat(Enumerable.Repeat("0", cantidad_Digitos));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - cantidad_Digitos, cantidad_Digitos);
                    modelo.NumeroDocumento = numeroVenta;
                    await _dbContext.AddAsync(modelo);
                    await _dbContext.SaveChangesAsync();
                    ventaGenerada = modelo;
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
                return ventaGenerada;
            }

        }
    }
}
