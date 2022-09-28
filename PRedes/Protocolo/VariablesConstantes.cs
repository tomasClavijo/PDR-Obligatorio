using System.Text;

namespace Protocolo
{
    public static class VariablesConstantes
    {
        public static readonly int FixedDataSize = 4;

        public const int FixedFileSize = 8;
        public const int MaxPacketSize = 32768; //32KB

        public const int Header = 3;
        public const int Comand = 2;
        public const int Length = 4;


        public static long CalculateFileParts(long fileSize)
        {
            var fileParts = fileSize / MaxPacketSize;
            return fileParts * MaxPacketSize == fileSize ? fileParts : fileParts + 1;
        }
    }
}
