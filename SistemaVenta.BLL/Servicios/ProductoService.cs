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
using Microsoft.EntityFrameworkCore;

namespace SistemaVenta.BLL.Servicios
{
    public class ProductoService : IProductoService
    {
        private readonly IGenericRepository<Producto> _productorepositorio;
        private readonly IMapper _mapper;

        public ProductoService(IGenericRepository<Producto> productorepositorio, IMapper mapper)
        {
            _productorepositorio = productorepositorio;
            _mapper = mapper;
        }

        public async Task<List<ProductoDTO>> Lista()
        {
            try
            {
                var queryProducto = await _productorepositorio.Consultar();

                var listaProducto = queryProducto.Include(cat => cat.IdCategoriaNavigation).ToList();

                return _mapper.Map<List<ProductoDTO>>(listaProducto.ToList());
            }
            catch
            {
                throw;
            }
        }

        public async Task<ProductoDTO> Crear(ProductoDTO modelo)
        {
            try
            {
                var productoCreado = await _productorepositorio.Crear(_mapper.Map<Producto>(modelo));

                if (productoCreado.IdProducto == null)
                {
                    throw new TaskCanceledException("No se pudo crear");
                }

                return _mapper.Map<ProductoDTO>(productoCreado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(ProductoDTO modelo)
        {
            try
            {
                var productoModelo = _mapper.Map<Producto>(modelo);

                var productoEncontrado = await _productorepositorio.Obtener(u => u.IdProducto == productoModelo.IdProducto);

                if (productoEncontrado.IdProducto == null)
                {
                    throw new TaskCanceledException("El producto no existe");
                }

                productoEncontrado.Nombre = productoModelo.Nombre;
                productoEncontrado.IdCategoria = productoModelo.IdCategoria;
                productoEncontrado.Stock = productoModelo.Stock;
                productoEncontrado.Precio = productoModelo.Precio;
                productoEncontrado.EsActivo = productoModelo.EsActivo;

                bool respuesta = await _productorepositorio.Editar(productoEncontrado);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se puedo editar el producto");
                }

                return respuesta;
            }

            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                var productoEncontrado = await _productorepositorio.Obtener(p => p.IdProducto == id);

                if (productoEncontrado == null)
                {
                    throw new TaskCanceledException("El producto no existe");

                }

                bool respuesta = await _productorepositorio.Eliminar(productoEncontrado);

                if (!respuesta)
                {
                    throw new TaskCanceledException("No se puedo eliminar el producto");
                }

                return respuesta;

            }
            catch
            {
                throw;
            }
        }


    }
}
