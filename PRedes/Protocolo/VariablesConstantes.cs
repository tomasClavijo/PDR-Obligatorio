﻿using System.Text;

namespace Protocol
{
    public static class VariablesConstantes
    {
        public static readonly int FixedDataSize = 4;

        public const int FixedFileSize = 8;
        public const int MaxPacketSize = 32768; //32KB

        public static long CalculateFileParts(long fileSize)
        {
            var fileParts = fileSize / MaxPacketSize;
            return fileParts * MaxPacketSize == fileSize ? fileParts : fileParts + 1;
        }
    }
}
