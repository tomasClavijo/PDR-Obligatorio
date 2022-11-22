using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LKAdin
{
    public class Usuario
    {
        String _userName;
        String _name;
        String _password;
        
        public String UserName
        {
            get { return _userName; }
            set
            {
                if (!string.IsNullOrEmpty(value)){
                    _userName = value;
                }
                else
                {
                    throw new ArgumentException("El campo nombre de usuario es obligatorio");
                }
            }
        }
        
        public String Name
        {
            get { return _name; }
            set
            {
                if (!string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"^[A-Za-z ]*$"))
                {
                    _name = value;
                }
                else
                {
                    throw new ArgumentException("Nombre no valido");
                }
            }
        }
        public String Password {
            get { return _password; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _password = value;
                }
                else
                {
                    throw new ArgumentException("La password es un campo obligatorio");
                }
            }
        }
        public Guid guid { get; set; }

        public bool Equals(Usuario user)
        {
            return this.UserName.Equals(user.UserName);
        }


    }
}
