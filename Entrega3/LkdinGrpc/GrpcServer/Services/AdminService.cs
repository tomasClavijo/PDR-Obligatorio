using Grpc.Core;
using System.Threading.Tasks;
using System;
using LKAdin;
using System.Collections.Generic;

namespace GrpcServer.Services
{
    public class AdminService : Admin.AdminBase
    {

        public override Task<MessageReply> PostUser(UserDTO request, ServerCallContext context)
        {
            Controlador controlador = Controlador.GetInstance();
            string message = "Usuario creado correctamente";
            try
            {
                controlador.AltaUsuario(request.Name, request.Password, request.Username);
            }
            catch (Exception)
            {
                message = "El usuario ya existe";
            }
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> PostPerfil(PerfilDTO request, ServerCallContext context)
        {
            Controlador controlador = Controlador.GetInstance();
            string message = "Perfil creado correctamente";
            try
            {
                List<String> habilidades = new List<string>();
                foreach (string habilidad in request.Habilidades)
                {
                    habilidades.Add(habilidad);
                }
                Usuario user = controlador.GetUsuario(request.Username);
                controlador.CrearPerfil(user, request.Descripcion, habilidades);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> PatchPerfil(PerfilDTO request, ServerCallContext context)
        {
            Controlador controlador = Controlador.GetInstance();
            string message = "Perfil modificado correctamente";
            try
            {
                List<String> habilidades = new List<string>();
                foreach (string habilidad in request.Habilidades)
                {
                    habilidades.Add(habilidad);
                }
                Usuario user = controlador.GetUsuario(request.Username);
                controlador.EditarPerfil(user, request.Descripcion, habilidades);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> DeletePerfil(MessageReply request, ServerCallContext context)
        {
            Controlador controlador = Controlador.GetInstance();
            string message = "Perfil eliminado correctamente";
            try
            {
                controlador.BorrarPerfil(request.Message);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> PatchUser(UserPatchDTO request, ServerCallContext context)
        {
            Controlador controlador = Controlador.GetInstance();
            string message = "Usuario modificado correctamente";
            try
            {
                controlador.EditarUsuario(request.Name, request.Password, request.Username, request.NewUsername);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> DeleteUser(MessageReply username, ServerCallContext context)
        {
            Controlador controlador = Controlador.GetInstance();
            string message = "Usuario eliminado correctamente";
            try
            {
                controlador.BajaUsuario(username.Message);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<MessageReply> DeletePicture(MessageReply username, ServerCallContext context)
        {
            Controlador controlador = Controlador.GetInstance();
            string message = "Foto del perfil eliminada correctamente";
            try
            {
                controlador.EliminarFoto(username.Message);
            }
            catch (Exception e)
            {
                message = e.Message;
            }
            return Task.FromResult(new MessageReply { Message = message });
        }

        public override Task<UserList> GetUsers(Empty empty, ServerCallContext context)
        {
            Controlador controlador = Controlador.GetInstance();
            List<Usuario> message = controlador.GetUsuarios();
            var request = new UserList();
            foreach (Usuario usuario in message)
            {
                request.Users.Add(new UserDTO { Name = usuario.Name,
                                           Password = usuario.Password,
                                           Username = usuario.UserName});
            }
            return Task.FromResult(request);
        }
        
        public override Task<PerfilList> GetPerfiles(Empty empty, ServerCallContext context)
        {
            Controlador controlador = Controlador.GetInstance();
            List<Perfil> message = controlador.GetPerfiles();
            PerfilList request = new PerfilList();
            foreach (Perfil usuario in message)
            {
                var perfil = new PerfilDTO
                {
                    Username = usuario.Usuario.UserName,
                    Descripcion = usuario.Descripcion,
                    Habilidades = { usuario.Habilidades }
                };
                request.Perfiles.Add(perfil);
            }
            return Task.FromResult(request);
        }
    }
}
