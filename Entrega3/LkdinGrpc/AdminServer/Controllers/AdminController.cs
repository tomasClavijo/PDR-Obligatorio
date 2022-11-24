using System;
using System.Threading.Tasks;
using AdminServer.NewFolder;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Protocolo;

namespace AdminServer.Controllers
{
    [Route("admin")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private Admin.AdminClient client;
        private string grpcURL;


        static readonly GestorConfig gestorConfig = new GestorConfig();
        public AdminController()
        {  
            AppContext.SetSwitch(
                  "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            grpcURL = gestorConfig.ReadSettings("GrpcURL");
        }

        [HttpPost("users")]
        public async Task<ActionResult> PostUser([FromBody] UserDTO user)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.PostUserAsync(user);
            return Ok(reply.Message);
        }

        [HttpDelete("users")]
        public async Task<ActionResult> DeleteUser([FromQuery] String userName)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteUserAsync(new MessageReply { Message = userName });
            return Ok(reply.Message);
        }

        [HttpPatch("users")]
        public async Task<ActionResult> PatchUser([FromBody] UserPatchDTO user)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.PatchUserAsync(user);
            return Ok(reply.Message);
        }

        [HttpGet("users")]
        public async Task<ActionResult> GetUser()
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.GetUsersAsync(new Empty());
            return Ok(reply.Users);
        }

        [HttpPost("perfiles")]
        public async Task<ActionResult> PostPerfil([FromBody] PerfilWebDTO perfil)
        {
            var perfilDTO = new PerfilDTO
            {
                Username = perfil.Username,
                Descripcion = perfil.Descripcion,
                Habilidades = { perfil.Habilidades }
            };
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.PostPerfilAsync(perfilDTO);
            return Ok(reply.Message);
        }

        [HttpDelete("perfiles")]
        public async Task<ActionResult> DeletePerfil([FromQuery] String userName)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeletePerfilAsync(new MessageReply { Message = userName });
            return Ok(reply.Message);
        }

        [HttpPatch("perfiles")]
        public async Task<ActionResult> PatchPerfil([FromBody] PerfilWebDTO perfil)
        {
            var perfilDTO = new PerfilDTO
            {
                Username = perfil.Username,
                Descripcion = perfil.Descripcion,
                Habilidades = { perfil.Habilidades }
            };
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.PatchPerfilAsync(perfilDTO);
            return Ok(reply.Message);
        }

        [HttpGet("perfiles")]
        public async Task<ActionResult> GetPerfiles()
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.GetPerfilesAsync(new Empty());
            return Ok(reply.Perfiles);
        }

        [HttpDelete("fotos")]
        public async Task<ActionResult> DeleteFoto([FromQuery] String userName)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeletePictureAsync(new MessageReply { Message = userName });
            return Ok(reply.Message);
        }
    }
}
