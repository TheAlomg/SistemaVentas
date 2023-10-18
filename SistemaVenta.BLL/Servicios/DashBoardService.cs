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

namespace SistemaVenta.BLL.Servicios
{
    public class DashBoardService : IDashBoardService
    {
        private readonly IVentaRepository _ventaRepositorio;
        private readonly IGenericRepository<Producto> _productorepositorio;
        private readonly IMapper _mapper;

        public DashBoardService(IVentaRepository ventaRepositorio, IGenericRepository<Producto> productorepositorio, IMapper mapper)
        {
            _ventaRepositorio = ventaRepositorio;
            _productorepositorio = productorepositorio;
            _mapper = mapper;
        }

        private IQueryable<Venta> retornarVentas(IQueryable<Venta> tablaVenta, int restarCantidadDias)
        {
            DateTime? ultimaFecha = tablaVenta.OrderByDescending(v => v.FechaRegistro).Select(v => v.FechaRegistro).First();

            ultimaFecha = ultimaFecha.Value.AddDays(restarCantidadDias);

            return tablaVenta.Where(v => v.FechaRegistro.Value.Date >=  ultimaFecha.Value.Date);
        }


        private async Task<int> TotalVentasUltimaSemanas()
        {
            int total = 0;
            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if(_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventaQuery, -7);
                total = tablaVenta.Count();
            }
            return total;
        }

        private async Task<string> TotalIngresosUltimaSemanas()
        {
            decimal resultado = 0;


            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();

            if(_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventaQuery, -7);

                resultado = tablaVenta.Select(v => v.Total).Sum(v=> v.Value);
            }

            return Convert.ToString(resultado, new CultureInfo("es-CO"));
        }

        private async Task<int> TotalProductos()
        {
            IQueryable<Producto> productoQuery = await _productorepositorio.Consultar();

            int total = productoQuery.Count();
            return total;
        }

        private async Task<Dictionary<string, int>> VentasUltimaSemanas()
        {
            Dictionary<string, int> resultado = new Dictionary<string, int>();

            IQueryable<Venta> _ventaQuery = await _ventaRepositorio.Consultar();


            if(_ventaQuery.Count() > 0)
            {
                var tablaVenta = retornarVentas(_ventaQuery, -7);

                resultado = tablaVenta
                    .GroupBy(v => v.FechaRegistro.Value.Date).OrderBy(g => g.Key)
                    .Select(dv => new { fecha = dv.Key.ToString("dd/MM/yyyy"), total = dv.Count() })
                    .ToDictionary(keySelector:r => r.fecha, elementSelector: r => r.total);
            }

            return resultado;
        }

        public async Task<DashBoardDTO> Resumen()
        {
            DashBoardDTO vmDashBoard = new DashBoardDTO();
            try
            {
                vmDashBoard.TotalVentas = await TotalVentasUltimaSemanas();
                vmDashBoard.TotalIngresos = await TotalIngresosUltimaSemanas();
                vmDashBoard.TotalProductos = await TotalProductos();


                List<VentaSemanaDTO> listaVentaSemana = new List<VentaSemanaDTO>();

                foreach(KeyValuePair<string,int> item in await VentasUltimaSemanas())
                {
                    listaVentaSemana.Add(new VentaSemanaDTO()
                    {
                        Fecha = item.Key,
                        Total = item.Value,
                    });
                }

                vmDashBoard.VentasUltimaSemana = listaVentaSemana;
            }
            catch
            {
                throw;
            }
            return vmDashBoard;
        }
    }
}
