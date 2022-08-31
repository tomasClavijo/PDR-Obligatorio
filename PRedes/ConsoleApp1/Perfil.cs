using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace LKdin
{
    public class Perfil : Usuario
    {
        List<String> habilidades { get; set; }

        String descripcion { get; set; }

        Bitmap imagen { get; set; }

    }
}
