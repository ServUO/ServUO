/***************************************************************************
*                            BinaryMemoryWriter.cs
*                            -------------------
*   begin                : May 1, 2002
*   copyright            : (C) The RunUO Software Team
*   email                : info@runuo.com
*
*   $Id: BinaryMemoryWriter.cs 37 2006-06-19 17:28:24Z mark $
*
***************************************************************************/








/***************************************************************************
*
*   This program is free software; you can redistribute it and/or modify
*   it under the terms of the GNU General Public License as published by
*   the Free Software Foundation; either version 2 of the License, or
*   (at your option) any later version.
*
***************************************************************************/
using System;
using System.IO;

namespace Server
{
    public sealed class BinaryMemoryWriter : BinaryFileWriter
    {
        private static byte[] indexBuffer;
        private readonly MemoryStream stream;
        public BinaryMemoryWriter()
            : base(new MemoryStream(512), true)
        {
            this.stream = this.UnderlyingStream as MemoryStream;
        }

        protected override int BufferSize
        {
            get
            {
                return 512;
            }
        }
        public int CommitTo(SequentialFileWriter dataFile, SequentialFileWriter indexFile, int typeCode, int serial)
        {
            this.Flush();

            byte[] buffer = this.stream.GetBuffer();
            int length = (int)this.stream.Length;

            long position = dataFile.Position;

            dataFile.Write(buffer, 0, length);

            if (indexBuffer == null)
            {
                indexBuffer = new byte[20];
            }

            indexBuffer[0] = (byte)(typeCode);
            indexBuffer[1] = (byte)(typeCode >> 8);
            indexBuffer[2] = (byte)(typeCode >> 16);
            indexBuffer[3] = (byte)(typeCode >> 24);

            indexBuffer[4] = (byte)(serial);
            indexBuffer[5] = (byte)(serial >> 8);
            indexBuffer[6] = (byte)(serial >> 16);
            indexBuffer[7] = (byte)(serial >> 24);

            indexBuffer[8] = (byte)(position);
            indexBuffer[9] = (byte)(position >> 8);
            indexBuffer[10] = (byte)(position >> 16);
            indexBuffer[11] = (byte)(position >> 24);
            indexBuffer[12] = (byte)(position >> 32);
            indexBuffer[13] = (byte)(position >> 40);
            indexBuffer[14] = (byte)(position >> 48);
            indexBuffer[15] = (byte)(position >> 56);

            indexBuffer[16] = (byte)(length);
            indexBuffer[17] = (byte)(length >> 8);
            indexBuffer[18] = (byte)(length >> 16);
            indexBuffer[19] = (byte)(length >> 24);

            indexFile.Write(indexBuffer, 0, indexBuffer.Length);

            this.stream.SetLength(0);

            return length;
        }
    }
}