using Protocolo;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LKAdin
{
    public class Controlador
    {
        static readonly GestorConfig settingsManager = new GestorConfig();
        private static Controlador instance = null;
        private static readonly object singletonlock = new object();

        public static Controlador GetInstance()
        {
            lock (singletonlock)
            {

                if (instance == null)
                    instance = new Controlador();
            }
            return instance;
        }



        List<Usuario> Usuarios { get; set; }
        List<Perfil> Perfiles { get; set; }
        List<Mensajeria> Mensajes { get; set; }


        public Guid AltaUsuario(String nombre, String password, String userName)
        {
            Usuario usuario = new Usuario();
            usuario.Name = nombre;
            usuario.Password = password;
            usuario.UserName = userName;
            usuario.guid = Guid.NewGuid();

            bool found = false;

            lock (Usuarios)
            {

                foreach (Usuario u in Usuarios)
                {
                    found = u.Equals(usuario);

                    if (found)
                        break;
                }
                if (found)
                {
                    throw new ArgumentException("El usuario ya existe");
                }
                Usuarios.Add(usuario);
            }
            return usuario.guid;

        }

        public void BajaUsuario(String username)
        {
            Usuario usuario = new Usuario();
            usuario.UserName = username;
            bool found = false;

            lock (Usuarios)
            {
                foreach (Usuario u in Usuarios)
                {
                    found = u.Equals(usuario);

                    if (found)
                    {
                        foreach  (Perfil p in Perfiles)
                        {
                            if(p.Usuario.UserName == usuario.UserName)
                            {
                                Perfiles.Remove(p);
                                break;
                            }
                        }
                        Usuarios.Remove(u);
                        break;
                    }
                }
                if (!found)
                {
                    throw new ArgumentException("El usuario no existe");
                }
            }
        }

        public void EditarUsuario(String nombre, String password, String userName, String userNameNuevo)
        {
            Usuario usuario = new Usuario();
            usuario.UserName = userName;
            Usuario nuevo = new Usuario();
            nuevo.Name = userNameNuevo;
            bool found = false;

            lock (Usuarios)
            {
                if (!String.IsNullOrEmpty(userNameNuevo))
                {

                    foreach (Usuario u in Usuarios)
                    {
                        found = u.Equals(nuevo);

                        if (found)
                        {
                            throw new ArgumentException("El nuevo username ya existe");
                        }
                    }
                }
                foreach (Usuario u in Usuarios)
                {
                    found = u.Equals(usuario);

                    if (found)
                    {
                        u.Name = (String.IsNullOrEmpty(nombre))? u.Name : nombre;
                        u.Password = (String.IsNullOrEmpty(password)) ? u.Password : password;
                        u.UserName = (String.IsNullOrEmpty(userNameNuevo)) ? u.UserName : userNameNuevo;
                    }
                }
                if (!found)
                {
                    throw new ArgumentException("El usuario no existe");
                }
            }
        }

        public List<Usuario> GetUsuarios()
        {
            return Usuarios;
        }

        public List<Perfil> GetPerfiles()
        {
            return Perfiles;
        }

        public Usuario GetUsuario(string userName)
        {
            foreach (Usuario u in Usuarios)
            {
                if (u.UserName == userName)
                {
                    return u;
                }
            }
            throw new ArgumentException("El usuario no existe");
        }


        public void CrearPerfil(Usuario usuario, String descripcion, List<String> habilidades)
        {
            Perfil perfil = new Perfil(usuario);
            perfil.Descripcion = descripcion;
            perfil.Habilidades = habilidades;
            lock (Perfiles)
            {
                bool yaExiste = Perfiles.Contains(perfil);
                if (!yaExiste)
                {
                    Perfiles.Add(perfil);
                }
                else
                {
                    throw new ArgumentException("El perfil ya existe");
                }
            }
        }

        public void EditarPerfil(Usuario usuario, String descripcion, List<String> habilidades)
        {
            Perfil perfil = new Perfil(usuario);
            perfil.Descripcion = descripcion;
            perfil.Habilidades = habilidades;
            lock (Perfiles)
            {
                bool yaExiste = Perfiles.Contains(perfil);
                if (!yaExiste)
                {
                    throw new ArgumentException("El perfil no existe");
                }
                foreach (Perfil p in Perfiles)
                {
                    if (p.Equals(perfil))
                    {
                        p.Descripcion = descripcion;
                        p.Habilidades = habilidades;
                        return;
                    }
                }
            }
        }

        public void BorrarPerfil(string username)
        {
            lock (Perfiles)
            {
                foreach (Perfil p in Perfiles)
                {
                    if (p.Usuario.UserName == username)
                    {
                        Perfiles.Remove(p);
                        return;
                    }
                }
                throw new ArgumentException("El perfil no existe");
            }
        }

        public void EliminarFoto(string username)
        {
            string rutaImagenes = settingsManager.ReadSettings(ConfigServidor.PictureFolder);
            string rutaImagen = rutaImagenes + "\\" + username + ".jpg";
            PropiedadesArchivo pA = new PropiedadesArchivo();
            bool tieneFoto = pA.FileExists(rutaImagen);
            if (!tieneFoto)
            {
                throw new ArgumentException("El usuario no tiene foto asociada");
            }
            File.Delete(rutaImagen);
        }

        public String BuscarPerfilId(String idPerfil)
        {

            StringBuilder retorno = new StringBuilder();
            lock (Perfiles)
            {
                for (int i = 0; i < Perfiles.Count; i++)
                {
                    if (Perfiles[i].Usuario.UserName.Equals(idPerfil))
                    {
                        retorno.Append(Perfiles[i].ToString());
                        retorno.Append("---------------------------");
                    }
                }
            }

            return retorno.ToString();
        }

        public Perfil BuscarPerfilGuid(Guid guidPerfil)
        {
            Perfil retorno = null;
            lock (Perfiles)
            {
                for (int i = 0; i < Perfiles.Count; i++)
                {
                    if (Perfiles[i].Usuario.guid.Equals(guidPerfil))
                    {
                        retorno = Perfiles[i];
                        break;
                    }
                }
            }

            if (retorno is not null)
            {
                return retorno;
            }
            throw new ArgumentException("Usted no tiene perfil, para realizar esta operacion");
        }

        public Perfil BuscarPerfilUserId(String idPerfil)
        {
            Perfil retorno = null;
            lock (Perfiles)
            {
                for (int i = 0; i < Perfiles.Count; i++)
                {
                    if (Perfiles[i].Usuario.UserName.Equals(idPerfil))
                    {
                        retorno = Perfiles[i];
                    }
                }
            }

            if (retorno is not null)
            {
                return retorno;
            }
            throw new ArgumentException("El perfil no existe");
        }

        public String BuscarUsuarioNombre(String Nombre)
        {
            StringBuilder retorno = new StringBuilder();
            lock (Perfiles)
            {
                for (int i = 0; i < Perfiles.Count; i++)
                {
                    if (Perfiles[i].Usuario.Name.ToLower().Contains(Nombre.ToLower()))
                    {
                        retorno.Append(Perfiles[i].ToString());
                        retorno.AppendLine("---------------------------");
                    }
                }
            }

            return retorno.ToString();
        }

        public Usuario BuscarUsuarioGuid(Guid guid)
        {

            Usuario usuario = null;
            lock (Usuarios)
            {
                for (int i = 0; i < Usuarios.Count; i++)
                {
                    if (Usuarios[i].guid.Equals(guid))
                    {
                        usuario = Usuarios[i];
                        break;
                    }
                }
            }

            if (usuario is not null)
            {
                return usuario;
            }

            throw new ArgumentException("No está autorizado a realizar esta operacion");

        }

        public String BuscarPorHabilidad(String[] habilidades)
        {
            StringBuilder coincidente = new StringBuilder();
            lock (Perfiles)
            {
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
                        if (coincidencias == habilidades.Length)
                        {
                            coincidente.Append(perfil.ToString());
                            coincidente.Append("---------------------------");

                        }
                    }
                }
            }

            return coincidente.ToString();
        }

        public String MensajesRecibidos(Perfil receptor, bool sinLeer)
        {
            StringBuilder mensajesRecibidos = new StringBuilder();
            lock (Mensajes)
            {
                for (int i = 0; i < Mensajes.Count; i++)
                {
                    Mensajeria mensaje = Mensajes[i];
                    if (mensaje.Receptor.Equals(receptor) && ((!mensaje.Leido && sinLeer) || (mensaje.Leido && !sinLeer)))
                    {
                        if (!mensaje.Leido)
                        {
                            mensaje.Leido = true;
                        }
                        mensajesRecibidos.Append(mensaje.ToString());
                    }
                }
            }

            return mensajesRecibidos.ToString();
        }

        public void EnviarMensaje(string mensaje, Perfil emisor, Perfil receptor)
        {
            Mensajeria enviarMensaje = new Mensajeria();
            enviarMensaje.mensajes = mensaje;
            enviarMensaje.Emisor = emisor;
            enviarMensaje.Receptor = receptor;
            lock (Mensajes)
            {
                Mensajes.Add(enviarMensaje);
            }
        }

        public Controlador()
        {
            Usuarios = new List<Usuario>();
            Perfiles = new List<Perfil>();
            Mensajes = new List<Mensajeria>();
        }
    }
}
