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

namespace SistemaVenta.BLL.Servicios
{
    public class CategoriaService: ICategoriaService
    {
        private readonly IGenericRepository<Categoria> _categoriarepositorio;
        private readonly IMapper _mapper;

        public CategoriaService(IGenericRepository<Categoria> categoriarepositorio, IMapper mapper)
        {
            _categoriarepositorio = categoriarepositorio;
            _mapper = mapper;
        }

        public async Task<List<CategoriaDTO>> Lista()
        {
            try
            {
                var listaCategoria = await _categoriarepositorio.Consultar();

                return _mapper.Map<List<CategoriaDTO>>(listaCategoria.ToList());
            }
            catch
            {
                throw;
            }
        }
    }
}
