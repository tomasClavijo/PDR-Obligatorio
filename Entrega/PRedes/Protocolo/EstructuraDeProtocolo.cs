using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Protocolo
{
    public static class EstructuraDeProtocolo
    {
        public static async Task envioAsync(String tipo, String comando, int largo, String mensaje, ManejoDataSocket socket)
        {

            byte[] tipoEnBytes = Encoding.UTF8.GetBytes(tipo);
            byte[] codigoEnBytes = Encoding.UTF8.GetBytes(comando);
            byte[] largoEnBytes = BitConverter.GetBytes(largo);
            byte[] mensajeEnBytes = Encoding.UTF8.GetBytes(mensaje);

            var taskTipo =  socket.SendAsync(tipoEnBytes);
            var taskCodigo =  socket.SendAsync(codigoEnBytes);
            var taskLargo = socket.SendAsync(largoEnBytes);
            var taskMensaje = socket.SendAsync(mensajeEnBytes);
            await Task.WhenAll(taskTipo, taskCodigo, taskLargo, taskMensaje);

        }


        public static async Task<List<string>> reciboAsync(ManejoDataSocket manejo)
        {
            List<String> retorno = new List<string>();
            byte[] tiopoMensaje = await manejo.ReciveAsync(VariablesConstantes.Header);
            String tipo = Encoding.UTF8.GetString(tiopoMensaje);
            byte[] comandMensaje = await manejo.ReciveAsync(VariablesConstantes.Comand);
            String comando = Encoding.UTF8.GetString(comandMensaje);
            byte[] largoMensaje = await manejo.ReciveAsync(VariablesConstantes.Length);
            int largo = BitConverter.ToInt32(largoMensaje);
            byte[] mensaje = await manejo.ReciveAsync(largo);
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
