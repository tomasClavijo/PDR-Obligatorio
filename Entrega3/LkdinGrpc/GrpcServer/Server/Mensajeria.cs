using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKAdin
{
    public class Mensajeria
    {
        public String mensajes { get; set; }
        public Perfil Emisor { get; set; }
        public Perfil Receptor { get; set; }
        public bool Leido { get; set; }

        public override String ToString()
        {
            return "Mensaje de: \n" + Emisor.Usuario.Name + "\n" + mensajes; 
        }
    }
}
