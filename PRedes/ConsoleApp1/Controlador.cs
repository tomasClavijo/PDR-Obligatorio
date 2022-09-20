using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LKdin
{
    public class Controlador
    {
        List<Usuario> Usuarios { get; set; }
        List<Perfil> Perfiles { get; set; }
        List<Mensajeria> Mensajes { get; set; }


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
            if (!Perfiles.Contains(perfil))
            {
                Perfiles.Add(perfil);
            }
            else
            {
                throw new Exception("El perfil ya existe");
            }
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
            throw new Exception();
        }

        public Perfil BuscarPerfilId(String idPerfil)
        {
            
            for (int i = 0; i < Perfiles.Count; i++)
            {
                if (Perfiles[i].UserId.Equals(idPerfil))
                {
                    return Perfiles[i];
                }
            }
            throw new Exception();
        }

        public void AsociarFoto(Perfil perfil, String foto)
        {
            perfil.Imagen = new Bitmap(foto);
        }

        public List<Usuario> BuscarUsuarioNombre(String Nombre){
            List<Usuario> retorno = new List<Usuario>();
            Usuario usuario = new Usuario();
            usuario.Name = Nombre;
            for (int i = 0; i < Usuarios.Count; i++)
            {
                if (Usuarios[i].Name.Equals(usuario.Name))
                {
                    retorno.Add( Usuarios[i]);
                }
            }
            return retorno;
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

        public List<Mensajeria> MensajesRecibidos (Perfil receptor)
        {
            List<Mensajeria> mensajesRecibidos = new List<Mensajeria>(); 
            for (int i = 0; i < Mensajes.Count; i++)
            {
                Mensajeria mensaje = Mensajes[i];
                if (mensaje.Receptor.Equals(receptor))
                {
                    
                    mensajesRecibidos.Add(mensaje);
                    mensaje.Leido = true;
                    
                }
            }
            return mensajesRecibidos;
        }

        public  void EnviarMensaje (string mensaje, Perfil emisor, Perfil receptor)
        {
            Mensajeria enviarMensaje = new Mensajeria();
            enviarMensaje.mensajes = mensaje;
            enviarMensaje.Emisor = emisor;
            enviarMensaje.Receptor = receptor;
            Mensajes.Add(enviarMensaje);
        }

        public Controlador()
        {
            Usuarios = new List<Usuario>();
            Perfiles = new List<Perfil>();
        }
    }
}
