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
        public List<String> _habilidades;
        public String _descripcion;

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
            this.UserName = usuario.UserName;
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
                sb.AppendLine("  -" + Habilidades[i]);
            }
            return sb.ToString();
        }

        public override String ToString()
        {
            return this.Name + "\n" +
                "Descripcion: \n" + this.Descripcion + "\nHabilidades: \n" + habilidadesToString();

        }

    }
}
