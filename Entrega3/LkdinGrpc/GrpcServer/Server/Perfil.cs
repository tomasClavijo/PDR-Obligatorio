using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LKAdin
{
    public class Perfil
    {
        public List<String> _habilidades;
        public String _descripcion;

        public Usuario Usuario { get; set; }

        public List<String> Habilidades
        {
            get { return _habilidades; }
            set
            {
                if (value is not null && value.Count > 0)
                {
                    _habilidades = value;
                }
                else
                {
                    throw new ArgumentException("Debe ingresar al menos una habilidad");
                }
            }
        }

        public String Descripcion
        {
            get { return _descripcion; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _descripcion = value;
                }
                else
                {
                    throw new ArgumentException("El campo descripcion es obligatorio");
                }
            }
        }

        public Perfil(Usuario usuario)
        {
            Usuario = usuario;
        }
        
        public bool Equals(Perfil perfil)
        {
            return this.Usuario.guid.Equals(perfil.Usuario.guid) || this.Usuario.UserName.Equals(perfil.Usuario.UserName);
        }

        public String habilidadesToString()
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Habilidades.Count; i++)
            {
                sb.AppendLine("  -" + Habilidades[i]);
            }
            return sb.ToString();
        }

        public override String ToString()
        {
            return this.Usuario.Name + "\n" +
                "Descripcion: \n" + this.Descripcion + "\nHabilidades: \n" + habilidadesToString();

        }

    }
}
