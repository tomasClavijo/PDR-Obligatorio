using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKdin
{
    public class Controlador
    {
        List<Usuario> Usuarios { get; set; }


        public void AltaUsuario(String userId, String password)
        {
            Usuario usuario = new Usuario();
            usuario.UserId = userId;
            usuario.Password = password;
            Usuarios.Add(usuario);
        }

        public void CrearPerfil(Usuario usuario, String descripcion, List<String> habilidades){

        }

        public Usuario BuscarUsuarioId(String idUsuario)
        {
            return null;
        }
        public List<Usuario> BuscarUsuarioNombre(String Nombre){
            return null;
        }

        public List<Usuario> BuscarPorHabilidad(List<String> habilidades)
        {
            return null;
        }
    }
}
