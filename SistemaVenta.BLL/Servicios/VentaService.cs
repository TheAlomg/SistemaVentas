using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;
using SistemaVenta.BLL.Servicios.Contrato;
using SistemaVenta.DAL.Repositorio.Contrato;
using SistemaVenta.Model;
using SistemaVenta.DTO;
using System.Globalization;
using Microsoft.EntityFrameworkCore;

namespace SistemaVenta.BLL.Servicios
{
    public class VentaService : IVentaService
    {
        private readonly IVentaRepository _ventaRepositorio;
        private readonly IGenericRepository<DetallesVenta> _detalleVentarepositorio;
        private readonly IMapper _mapper;

        public VentaService(IVentaRepository ventaRepository, IGenericRepository<DetallesVenta> detalleVentarepositorio, IMapper mapper)
        {
            _ventaRepositorio = ventaRepository;
            _detalleVentarepositorio = detalleVentarepositorio;
            _mapper = mapper;
        }

        public async Task<VentaDTO> Registrar(VentaDTO modelo)
        {
            try
            {
                var ventaGenerada = await _ventaRepositorio.Registrar(_mapper.Map<Venta>(modelo));

                if(ventaGenerada.IdVenta == 0)
                {
                    throw new TaskCanceledException("No se pudo crear");
                }

                return _mapper.Map<VentaDTO>(ventaGenerada);
            }
            catch 
            {
                throw;
            }
        }

        public async Task<List<VentaDTO>> Historial(string buscarPor, string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _ventaRepositorio.Consultar();
            var ListaResultado = new List<Venta>();

            try
            {

                if(buscarPor == "Fecha")
                {
                    DateTime fech_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
                    DateTime fech_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

                    ListaResultado = await query.Where(v => v.FechaRegistro.Value.Date >= fech_Inicio.Date && 
                    v.FechaRegistro.Value.Date <= fech_Fin.Date).Include(dv => dv.DetallesVenta).ThenInclude(p=> p.IdProductoNavigation).ToListAsync();
                }
                else
                {
                    ListaResultado = await query.Where(v => v.NumeroDocumento == numeroVenta).Include(dv => dv.DetallesVenta).ThenInclude(p => p.IdProductoNavigation).ToListAsync();
                }
            }
            catch
            {
                throw;
            }
            return _mapper.Map<List<VentaDTO>>(ListaResultado);
        }

        public async Task<List<ReporteDTO>> Reporte(string fechaInicio, string fechaFin)
        {
            IQueryable<DetallesVenta> query = await _detalleVentarepositorio.Consultar();
            var ListaResultado = new List<DetallesVenta>();
            try
            {
                DateTime fech_Inicio = DateTime.ParseExact(fechaInicio, "dd/MM/yyyy", new CultureInfo("es-CO"));
                DateTime fech_Fin = DateTime.ParseExact(fechaFin, "dd/MM/yyyy", new CultureInfo("es-CO"));

                ListaResultado = await query
                    .Include(p => p.IdProductoNavigation)
                    .Include(v => v.IdVentaNavigation)
                    .Where(dv => 
                    dv.IdVentaNavigation.FechaRegistro.Value.Date >= fech_Inicio.Date&&
                    dv.IdVentaNavigation.FechaRegistro.Value.Date <= fech_Fin.Date).ToListAsync();
            }
            catch
            {
                throw;
            }
            return _mapper.Map<List<ReporteDTO>> (ListaResultado);
        }
    }
}
