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
using System.Windows.Markup;

namespace SistemaVenta.BLL.Servicios
{
    public class MenuService : IMenuService
    {
        private readonly IGenericRepository<Usuario> _usuariorepositorio;
        private readonly IGenericRepository<MenuRol> _menurolrepositorio;
        private readonly IGenericRepository<Menu> _menurepositorio;
        private readonly IMapper _mapper;

        public MenuService(IGenericRepository<Usuario> usuariorepositorio, IGenericRepository<MenuRol> menurolrepositorio, IGenericRepository<Menu> menurepositorio, IMapper mapper)
        {
            _usuariorepositorio = usuariorepositorio;
            _menurolrepositorio = menurolrepositorio;
            _menurepositorio = menurepositorio;
            _mapper = mapper;
        }

        public async Task<List<MenuDTO>> Lista(int idUsuario)
        {
            IQueryable<Usuario> tbUsuario = await _usuariorepositorio.Consultar(u => u.IdUsuario == idUsuario);
            IQueryable<MenuRol> tbMenuRol = await _menurolrepositorio.Consultar();
            IQueryable<Menu> tbMenu = await _menurepositorio.Consultar();



            try
            {
                IQueryable<Menu> tbResultado = (from u in tbUsuario join mr in tbMenuRol on u.IdRol equals mr.IdRol join
                                                m in tbMenu on mr.IdMenu equals m.IdMenu select m).AsQueryable();

                var listaMenus = tbResultado.ToList();

                return _mapper.Map<List<MenuDTO>>(listaMenus);
            }
            catch
            {
                throw;
            }
        }
    }
}
