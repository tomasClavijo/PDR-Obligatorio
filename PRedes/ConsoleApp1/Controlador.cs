using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
            Usuario usuario = new Usuario();
            usuario.UserId = idUsuario;
            for (int i = 0; i < Usuarios.Count; i++)
            {
                if (Usuarios[i].UserId.Equals(usuario.UserId))
                {
                    return Usuarios[i];
                }
            }
        }

        public List<Usuario> BuscarUsuarioNombre(String Nombre){
            Usuario usuario = new Usuario();
            usuario.Name = Nombre;
            for (int i = 0; i < Usuario.Count; i++)
            {
                if (Usuario[i].Name.Equals(usuario.Name))
                {
                    return Usuarios[i];
                }
            }
        }

        public List<Usuario> BuscarPorHabilidad(List<String> habilidades)
        {
            return null;
        }
    }
}
