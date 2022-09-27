using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocolo
{
    public static class StructuralMessage
    {
        public static void envio(String tipo, String comando, int largo, String mensaje, ManejoDataSocket socket)
        {

            byte[] tipoEnBytes = Encoding.UTF8.GetBytes(tipo);
            byte[] codigoEnBytes = Encoding.UTF8.GetBytes(comando);
            byte[] largoEnBytes = BitConverter.GetBytes(largo);
            byte[] mensajeEnBytes = Encoding.UTF8.GetBytes(mensaje);

            socket.Send(tipoEnBytes);
            socket.Send(codigoEnBytes);
            socket.Send(largoEnBytes);
            socket.Send(mensajeEnBytes);
        }


        public static List<String> recibo(ManejoDataSocket manejo)
        {
            List<String> retorno = new List<string>();
            byte[] tiopoMensaje = manejo.Recive(3);
            String tipo = Encoding.UTF8.GetString(tiopoMensaje);
            byte[] comandMensaje = manejo.Recive(2);
            String comando = Encoding.UTF8.GetString(comandMensaje);
            byte[] largoMensaje = manejo.Recive(4);
            int largo = BitConverter.ToInt32(largoMensaje);
            byte[] mensaje = manejo.Recive(largo);
            String mensajeString = Encoding.UTF8.GetString(mensaje);
            var mensajeDescomprimido = mensajeString.Split("|");
            retorno.Add(tipo);
            retorno.Add(comando);
            retorno.Add(largo.ToString());
            for (int i = 0; i < mensajeDescomprimido.Length; i++)
            {
                retorno.Add(mensajeDescomprimido[i]);
            }
            return retorno;
        }
    }
}
