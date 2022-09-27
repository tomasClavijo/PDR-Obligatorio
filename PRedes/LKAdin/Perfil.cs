using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKAdin
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
            this.guid = usuario.guid;
        }

        public bool Equals(Perfil perfil)
        {
            return this.guid.Equals(perfil.guid);
        }

        public String habilidadesToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Habilidades.Count; i++)
            {
                sb.AppendLine("  -"+Habilidades[i]);
            }
            return sb.ToString();
        }

        public  String ToString()
        {
            return this.Name + "\n" +
                "Descripcion: \n" + this.Descripcion + "\nHabilidades: \n" + habilidadesToString();

        }

    }
}
