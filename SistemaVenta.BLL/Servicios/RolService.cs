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
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> _rolrepositorio;
        private readonly IMapper _mapper;

        public RolService(IGenericRepository<Rol> rolrepositorio, IMapper mapper)
        {
            _rolrepositorio = rolrepositorio;
            _mapper = mapper;
        }

        public async Task<List<RolDTO>> Lista()
        {
            try
            {
                var listaRoles = await _rolrepositorio.Consultar();
                return _mapper.Map<List<RolDTO>>(listaRoles.ToList());

            }
            catch
            {
                throw;
            }
        }
    }
}
