using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKdin
{
    public class Perfil : Usuario
    {
        public List<String> Habilidades { get; set; }

        public String Descripcion { get; set; }

        public Bitmap Imagen { get; set; }

        public Perfil(Usuario usuario)
        {
            this.UserId = usuario.UserId;
            this.Name = usuario.Name;
            this.Password = usuario.Password;
        }

    }
}
