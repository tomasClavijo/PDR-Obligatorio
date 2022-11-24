using System.Collections.Generic;

namespace AdminServer.NewFolder
{
    public class PerfilWebDTO
    {
        public string Username { get; set; }
        public string Descripcion { get; set; }
        public IEnumerable<string> Habilidades { get; set; }
    }
}
