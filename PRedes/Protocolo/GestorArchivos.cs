using Protocolo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Protocolo
{
    public class GestorArchivos
    {
        private readonly ConvertorVariables _conversionHandler;
        private readonly PropiedadesArchivo _fileHandler;
        private readonly TratamientoArchivo _fileStreamHandler;
        private readonly ManejoDataSocket _socketHelper;

        public GestorArchivos(Socket socket)
        {
            _conversionHandler = new ConvertorVariables();
            _fileHandler = new PropiedadesArchivo();
            _fileStreamHandler = new TratamientoArchivo();
            _socketHelper = new ManejoDataSocket(socket);
        }

        public void SendFile(string path)
        {
            if (_fileHandler.FileExists(path))
            {
                var fileName = _fileHandler.GetFileName(path);
                // ---> Enviar el largo del nombre del archivo
                _socketHelper.Send(_conversionHandler.ConvertIntToBytes(fileName.Length));
                // ---> Enviar el nombre del archivo
                _socketHelper.Send(_conversionHandler.ConvertStringToBytes(fileName));

                // ---> Obtener el tamaño del archivo
                long fileSize = _fileHandler.GetFileSize(path);
                if (fileSize == 0)
                {
                    throw new ArgumentException("El archivo está vacio");
                }
                // ---> Enviar el tamaño del archivo
                var convertedFileSize = _conversionHandler.ConvertLongToBytes(fileSize);
                _socketHelper.Send(convertedFileSize);
                // ---> Enviar el archivo (pero con file stream)
                SendFileWithStream(fileSize, path);
            }
            else
            {
                throw new Exception("El arhivo no existe");
            }
        }

        public void ReceiveFile(string userName)
        {
            // ---> Recibir el largo del nombre del archivo
            int fileNameSize = _conversionHandler.ConvertBytesToInt(
                _socketHelper.Recive(VariablesConstantes.FixedDataSize));
            // ---> Recibir el nombre del archivo
            string fileName = _conversionHandler.ConvertBytesToString(_socketHelper.Recive(fileNameSize));
            // ---> Recibir el largo del archivo
            long fileSize = _conversionHandler.ConvertBytesToLong(
                _socketHelper.Recive(VariablesConstantes.FixedFileSize));
            // ---> Recibir el archivo
            ReceiveFileWithStreams(fileSize, fileName, userName);
        }

        private void SendFileWithStream(long fileSize, string path)
        {
            long fileParts = VariablesConstantes.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;  
         
            //Mientras tengo un segmento a enviar
            while (fileSize > offset)
            {
                byte[] data;
                //Es el último segmento?
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    //1- Leo de disco el último segmento
                    //2- Guardo el último segmento en un buffer
                    data = _fileStreamHandler.Read(path, offset, lastPartSize); //Puntos 1 y 2
                    offset += lastPartSize;
                }
                else
                {
                    //1- Leo de disco el segmento
                    //2- Guardo ese segmento en un buffer
                    data = _fileStreamHandler.Read(path, offset, VariablesConstantes.MaxPacketSize);
                    offset += VariablesConstantes.MaxPacketSize;
                }

                _socketHelper.Send(data); //3- Envío ese segmento a travez de la red
                currentPart++;
            }
        }

        private void ReceiveFileWithStreams(long fileSize, string fileName, string userName)
        {
            long fileParts = VariablesConstantes.CalculateFileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            //Mientras tengo partes para recibir
            while (fileSize > offset)
            {
                byte[] data;
                //1- Me fijo si es la ultima parte
                if (currentPart == fileParts)
                {
                    //1.1 - Si es, recibo la ultima parte
                    var lastPartSize = (int)(fileSize - offset);
                    data = _socketHelper.Recive(lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    //2.2- Si no, recibo una parte cualquiera
                    data = _socketHelper.Recive(VariablesConstantes.MaxPacketSize);
                    offset += VariablesConstantes.MaxPacketSize;
                }
                //3- Escribo esa parte del archivo a disco
                _fileStreamHandler.Write(userName+".jpg", data);
                currentPart++;
            }
        }
    }
}

