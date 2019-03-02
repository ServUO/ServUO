/* Copyright (C) 2013 Ian Karlinsey
 * 
 * UltimeLive is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * UltimaLive is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with UltimaLive.  If not, see <http://www.gnu.org/licenses/>. 
 */

using Server;
using System.Collections.Generic;

namespace UltimaLive
{
    public sealed class CRC
    {
        //CRC caching
        //[map][block]
        public static ushort[][] MapCRCs;

        public static void InvalidateBlockCRC(int map, int block)
        {
            MapCRCs[map][block] = ushort.MaxValue;
        }

        public static void Configure()
        {
            EventSink.WorldLoad += new WorldLoadEventHandler(OnLoad);
        }

        public static void OnLoad()
        {
            MapCRCs = new ushort[256][];

            //We need CRCs for every block in every map.  
            foreach (KeyValuePair<int, MapRegistry.MapDefinition> kvp in MapRegistry.Definitions)
            {
                int blocks = Server.Map.Maps[kvp.Key].Tiles.BlockWidth * Server.Map.Maps[kvp.Key].Tiles.BlockHeight;
                MapCRCs[kvp.Key] = new ushort[blocks];

                for (int j = 0; j < blocks; j++)
                {
                    MapCRCs[kvp.Key][j] = ushort.MaxValue;
                }
            }
        }

        /* Thank you http://en.wikipedia.org/wiki/Fletcher%27s_checksum
		 * Each sum is computed modulo 255 and thus remains less than 
		 * 0xFF at all times. This implementation will thus never 
		 * produce the checksum results 0x00FF, 0xFF00 or 0xFFFF.
		/**/
        public static ushort Fletcher16(byte[] data)
        {
            ushort sum1 = 0;
            ushort sum2 = 0;
            int index;
            for (index = 0; index < data.Length; ++index)
            {
                sum1 = (ushort)((sum1 + data[index]) % 255);
                sum2 = (ushort)((sum2 + sum1) % 255);
            }

            return (ushort)((sum2 << 8) | sum1);
        }
    }
}
