using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKdin
{
    public class Controlador
    {
        List<Usuario> Usuarios { get; set; }
        List<Perfil> Perfiles { get; set; }


        public void AltaUsuario(String userId, String password)
        {
            Usuario usuario = new Usuario();
            usuario.UserId = userId;
            usuario.Password = password;
            Usuarios.Add(usuario);
        }

        public void CrearPerfil(Usuario usuario, String descripcion, List<String> habilidades){
            Perfil perfil = new Perfil(usuario);
            perfil.Descripcion = descripcion;
            perfil.Habilidades = habilidades;
            Perfiles.Add(perfil);
        }

        public Usuario BuscarUsuarioId(String idUsuario)
        {
            return null;
        }

        public List<Usuario> BuscarUsuarioNombre(String Nombre){
            return null;
        }

        public List<Perfil> BuscarPorHabilidad(List<String> habilidades)
        {
            List<Perfil> coincidente = new List<Perfil>();
            for (int i = 0; i < Perfiles.Count; i++)
            {

                int coincidencias = 0;
                Perfil perfil = Perfiles[i];
                for (int j = 0; j < habilidades.Count; j++)
                {
                    for (int k = 0; k < perfil.Habilidades.Count; k++)
                    {
                        if (habilidades[j].Equals(perfil.Habilidades[k]))
                        {
                            coincidencias++;
                            break;
                        }
                    }
                    if(coincidencias == habilidades.Count)
                    {
                        coincidente.Add(perfil);
                    }
                }
            }
            return coincidente;
        }

        public Controlador()
        {
            Usuarios = new List<Usuario>();
            Perfiles = new List<Perfil>();
        }
    }
}
