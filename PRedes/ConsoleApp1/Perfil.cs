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
        List<String> habilidades { get; set; }

        String descripcion { get; set; }

        Bitmap imagen { get; set; }
    }
}
