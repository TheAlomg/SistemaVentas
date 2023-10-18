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
    public class UsuarioService : IUsuarioService
    {
        private readonly IGenericRepository<Usuario> _usuariorepositorio;
        private readonly IMapper _mapper;

        public UsuarioService(IGenericRepository<Usuario> usuariorepositorio, IMapper mapper)
        {
            _usuariorepositorio = usuariorepositorio;
            _mapper = mapper;
        }

        public async Task<List<UsuarioDTO>> Lista()
        {
            try
            {
                var queryUsuario = await _usuariorepositorio.Consultar();
                var listaUsuarios = queryUsuario.Include(rol => rol.IdRolNavigation).ToList();
                return _mapper.Map<List<UsuarioDTO>>(listaUsuarios);
            }
            catch
            {
                throw;
            }
        }

        public async Task<SessionDTO> ValidarCredenciales(string correo, string clave)
        {
            try
            {
                var queryUsuario = await _usuariorepositorio.Consultar(u => 
                u.Correo == correo &&
                u.Clave == clave);

                if (queryUsuario.FirstOrDefault() == null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }

                Usuario devolverUsuario = queryUsuario.Include(rol => rol.IdRolNavigation).First();

                return _mapper.Map<SessionDTO>(devolverUsuario);
            }
            catch 
            {
                throw;
            }
        }

        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            try
            {
                var usuarioCreado = await _usuariorepositorio.Crear(_mapper.Map<Usuario> (modelo));

                if(usuarioCreado.IdUsuario==0)
                {
                    throw new TaskCanceledException("No se pudo crear el usuario");
                }
                var query = await _usuariorepositorio.Consultar(u => u.IdUsuario == usuarioCreado.IdUsuario);

                usuarioCreado = query.Include(rol => rol.IdRolNavigation).First();

                return _mapper.Map<UsuarioDTO>(usuarioCreado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Editar(UsuarioDTO modelo)
        {
            try
            {
                var usuarioModelo = _mapper.Map<Usuario>(modelo);

                var usuarioEncontrado = await _usuariorepositorio.Obtener(u => u.IdUsuario == usuarioModelo.IdUsuario);

                if(usuarioEncontrado== null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }

                usuarioEncontrado.NombreCompleto = usuarioModelo.NombreCompleto;
                usuarioEncontrado.Correo = usuarioModelo.Correo;
                usuarioEncontrado.IdRol = usuarioModelo.IdRol;
                usuarioEncontrado.Clave = usuarioModelo.Clave;
                usuarioEncontrado.EsActivo = usuarioModelo.EsActivo;


                bool respuesta = await _usuariorepositorio.Editar(usuarioEncontrado);

                if(!respuesta) 
                {
                    throw new TaskCanceledException("No se pudo editar");
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
                var usuarioEncontrado = await _usuariorepositorio.Obtener(u=> u.IdUsuario== id);

                if(usuarioEncontrado== null)
                {
                    throw new TaskCanceledException("El usuario no existe");
                }

                bool respuesta = await _usuariorepositorio.Eliminar(usuarioEncontrado);
                if (!respuesta)
                {
                    throw new TaskCanceledException("No se pudo eliminar");
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
