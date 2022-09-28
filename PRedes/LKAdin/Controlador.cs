using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LKAdin
{
    public class Controlador
    {
        List<Usuario> Usuarios { get; set; }
        List<Perfil> Perfiles { get; set; }
        List<Mensajeria> Mensajes { get; set; }


        public (Guid,String) AltaUsuario(String nombre, String password, String userName)
        {
            Usuario usuario = new Usuario();
            try
            {
                usuario.Name = nombre;
                usuario.Password = password;
                usuario.UserName = userName;
                usuario.guid = Guid.NewGuid();
                bool found = false;
                //Controlamos que no se registren dos usuarios con el mismo nombre, al mismo tiempo
                lock (this)
                {
                    foreach (Usuario u in Usuarios)
                    {
                        found = u.Equals(usuario);
                    }
                    if (found)
                    {
                        return (Guid.Empty, "El usuario ya existe");
                    }
                    Usuarios.Add(usuario);
                    return (usuario.guid, "Usuario registrado correctamente");
                }
            }
            catch (ArgumentException e){
                return (Guid.Empty, e.Message);
            }
        }

        public void CrearPerfil(Usuario usuario, String descripcion, List<String> habilidades){
            Perfil perfil = new Perfil(usuario);
            perfil.Descripcion = descripcion;
            perfil.Habilidades = habilidades;
            if (!Perfiles.Contains(perfil))
            {
                lock (this)
                {
                    Perfiles.Add(perfil);
                }
            }
            else
            {
                throw new ArgumentException("El perfil ya existe");
            }
        }

        

        public Usuario BuscarUsuarioId(String idUsuario)
        {
            Usuario usuario = new Usuario();
            usuario.UserName = idUsuario;
            for (int i = 0; i < Usuarios.Count; i++)
            {
                if (Usuarios[i].UserName.Equals(usuario.UserName))
                {
                    return Usuarios[i];
                }
            }
            throw new Exception();
        }

        public String BuscarPerfilId(String idPerfil)
        {

            StringBuilder retorno = new StringBuilder();

            for (int i = 0; i < Perfiles.Count; i++)
            {
                if (Perfiles[i].UserName.Equals(idPerfil))
                {
                    retorno.Append(Perfiles[i].ToString());
                    retorno.Append("---------------------------");
                }
            }
            return retorno.ToString();
        }

        public Perfil BuscarPerfilGuid(Guid guidPerfil)
        {


            for (int i = 0; i < Perfiles.Count; i++)
            {
                if (Perfiles[i].guid.Equals(guidPerfil))
                {
                    return Perfiles[i];
                }
            }
            return null;
        }

        public Perfil BuscarPerfilUserId(String idPerfil)
        {

            for (int i = 0; i < Perfiles.Count; i++)
            {
                if (Perfiles[i].UserName.Equals(idPerfil))
                {
                    return Perfiles[i];
                }
            }
            return null;
        }

        public String BuscarUsuarioNombre(String Nombre){
            StringBuilder retorno = new StringBuilder();

            for (int i = 0; i < Perfiles.Count; i++)
            {
                if (Perfiles[i].Name.ToLower().Contains(Nombre.ToLower()))
                {
                    retorno.Append(Perfiles[i].ToString());
                    retorno.AppendLine("---------------------------");
                }
            }
            return retorno.ToString();
        }

        public Usuario BuscarUsuarioGuid(Guid guid)
        {
            
            Usuario usuario = new Usuario();
            usuario.guid = guid;
            for (int i = 0; i < Usuarios.Count; i++)
            {
                if (Usuarios[i].guid.Equals(usuario.guid))
                {
                    return Usuarios[i];
                }
            }
            throw new ArgumentException("No está autorizado a realizar esta operación");

        }

        public String BuscarPorHabilidad(String[] habilidades)
        {
            StringBuilder coincidente = new StringBuilder();
            for (int i = 0; i < Perfiles.Count; i++)
            {

                int coincidencias = 0;
                Perfil perfil = Perfiles[i];
                for (int j = 0; j < habilidades.Length; j++)
                {
                    for (int k = 0; k < perfil.Habilidades.Count; k++)
                    {
                        if (habilidades[j].Equals(perfil.Habilidades[k]))
                        {
                            coincidencias++;
                            break;
                        }
                    }
                    if(coincidencias == habilidades.Length)
                    {
                        coincidente.Append(perfil.ToString());
                        coincidente.Append("---------------------------");

                    }
                }
            }
            return coincidente.ToString();
        }

        public String MensajesRecibidos (Perfil receptor)
        {
            StringBuilder mensajesRecibidos = new StringBuilder(); 
            for (int i = 0; i < Mensajes.Count; i++)
            {
                Mensajeria mensaje = Mensajes[i];
                if (mensaje.Receptor.Equals(receptor) && !mensaje.Leido)
                {
                    
                    mensajesRecibidos.Append(mensaje.ToString());
                    mensaje.Leido = true;
                    
                }
            }
            return mensajesRecibidos.ToString();
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
            Mensajes = new List<Mensajeria>();
        }
    }
}
