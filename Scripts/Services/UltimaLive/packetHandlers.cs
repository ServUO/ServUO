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
using Server.Accounting;
using Server.Gumps;
using Server.Network;
using System;
using System.Collections.Generic;
using System.IO;

namespace UltimaLive
{
    public class UltimaLivePacketHandlers
    {
        public static void Initialize()
        {
            PacketHandlers.Register(0x3F, 0, true, new OnPacketReceive(UltimaLivePacketHandlers.ReceiveUltimaLiveCommand));
        }

        public static void ReceiveUltimaLiveCommand(NetState state, PacketReader pvSrc)
        {
            pvSrc.Seek(13, SeekOrigin.Begin);
            byte ultimaLiveCommand = pvSrc.ReadByte();

            switch (ultimaLiveCommand)
            {
                case 0xFF: //block query response
                {
                    HandleBlockQueryReply(state, pvSrc);
                }
                break;

                /*case 0xFE: //read client version of UltimaLive
                  {
                    pvSrc.Seek(15, SeekOrigin.Begin);
                    UInt16 majorVersion = pvSrc.ReadUInt16();
                    UInt16 minorVersion = pvSrc.ReadUInt16();
                    //Console.WriteLine(String.Format("Received UltimaLive version packet: {0}.{1} from {2}", majorVersion, minorVersion, state.Mobile.Name));
                    if (state != null && state.Mobile is PlayerMobile)
                    {
                      PlayerMobile player = (PlayerMobile)state.Mobile;
                      player.UltimaLiveMajorVersion = majorVersion;
                      player.UltimaLiveMinorVersion = minorVersion;
                    }
                  }
                  break;*/

                // Need to write in functionality for direct server edit commands
                // from players with accesslevel that is high enough.
                // This will be for future enhancements on the client end (Client Overlay Editor,
                // Pandora's Box Plugin, etc)
                default:
                {
                }
                break;
            }
        }

        /*
         * Whenever the client moves out of a block, we will ask the client to 
         * provide us with a list of blocks around it and their corresponding
         * block versions.  If any of them are different than the server's 
         * block versions, we'll know the client needs to be updated, and 
         * we'll send the appropriate blocks.
        /**/

        public static void HandleBlockQueryReply(NetState state, PacketReader pvSrc)
        {
            Mobile from = state.Mobile;
            //byte 000              -  cmd
            //byte 001 through 002  -  packet size
            pvSrc.Seek(3, SeekOrigin.Begin);            //byte 003 through 006  -  central block number for the query (block that player is standing in)
            uint blocknum = pvSrc.ReadUInt32();
            //UInt32 count = pvSrc.ReadUInt32();          //byte 007 through 010  -  number of statics in the packet (8 standard)
            /*pvSrc.Seek(11, SeekOrigin.Begin);
            ushort sequence;
            if(QueryClientHash.Sequences.TryGetValue(from, out sequence) && sequence==pvSrc.ReadUInt16())//crafted packet or wrong sequence?! discarded!
            {*/
            //byte 011 through 012  -  UltimaLive sequence number - we sent one out, did we get it back?
            //byte 013              -  UltimaLive command (0xFF is a block Query Response)
            pvSrc.Seek(14, SeekOrigin.Begin);           //byte 014              -  UltimaLive mapnumber
            int mapID = pvSrc.ReadByte();

            if (mapID != from.Map.MapID)
            {
                Console.WriteLine(string.Format("Received a block query response from {0} for map {1} but that player is on map {2}",
                    from.Name, mapID, from.Map.MapID));
                return;
            }

            ushort[] receivedCRCs = new ushort[25];     //byte 015 through 64   -  25 block CRCs
            for (int i = 0; i < 25; i++)
            {
                receivedCRCs[i] = pvSrc.ReadUInt16();
            }

            PushBlockUpdates((int)blocknum, mapID, receivedCRCs, from);
            //}
        }

        public static ushort GetBlockCrc(Point2D blockCoords, int mapID, ref byte[] landDataOut, ref byte[] staticsDataOut)
        {
            if (blockCoords.X < 0 || blockCoords.Y < 0 || (blockCoords.X) >= Map.Maps[mapID].Tiles.BlockWidth || (blockCoords.Y) >= Map.Maps[mapID].Tiles.BlockHeight)
            {
                return 0;
            }

            landDataOut = BlockUtility.GetLandData(blockCoords, mapID);


            staticsDataOut = BlockUtility.GetRawStaticsData(blockCoords, mapID);
            byte[] blockData = new byte[landDataOut.Length + staticsDataOut.Length];
            Array.Copy(landDataOut, 0, blockData, 0, landDataOut.Length);
            Array.Copy(staticsDataOut, 0, blockData, landDataOut.Length, staticsDataOut.Length);
            return CRC.Fletcher16(blockData);
        }

        public static void PushBlockUpdates(int block, int mapID, ushort[] recievedCRCs, Mobile from)
        {
            //Console.WriteLine("------------------------------------------Push Block Updates----------------------------------------");
            //Console.WriteLine("Map: " + mapID);
            //Console.WriteLine("Block: " + block);
            //Console.WriteLine("----------------------------------------------------------------------------------------------------");

            if (!MapRegistry.Definitions.ContainsKey(mapID))
            {
                Console.WriteLine("Received query for an invalid map.");
                return;
            }

            ushort[] Hashes = new ushort[25];
            TileMatrix tm = Map.Maps[mapID].Tiles;

            int blockX = ((block / tm.BlockHeight));
            int blockY = ((block % tm.BlockHeight));
            int wrapWidthInBlocks = MapRegistry.Definitions[mapID].WrapAroundDimensions.X >> 3;
            int wrapHeightInBlocks = MapRegistry.Definitions[mapID].WrapAroundDimensions.Y >> 3;
            int mapWidthInBlocks = MapRegistry.Definitions[mapID].Dimensions.X >> 3;
            int mapHeightInBlocks = MapRegistry.Definitions[mapID].Dimensions.Y >> 3;
            //Console.WriteLine("BlockX: " + blockX + " BlockY: " + blockY);
            //Console.WriteLine("Map Height in blocks: " + mapHeightInBlocks);
            //Console.WriteLine("Map Width in blocks: " + mapWidthInBlocks);
            //Console.WriteLine("Wrap Height in blocks: " + wrapHeightInBlocks);
            //Console.WriteLine("Wrap Width in blocks: " + wrapWidthInBlocks);

            if (block < 0 || block >= mapWidthInBlocks * mapHeightInBlocks)
            {
                return;
            }

            byte[] buf = new byte[2];

            for (int x = -2; x <= 2; x++)
            {
                int xBlockItr = 0;
                if (blockX < wrapWidthInBlocks)
                {
                    xBlockItr = (blockX + x) % wrapWidthInBlocks;
                    if (xBlockItr < 0)
                    {
                        xBlockItr += wrapWidthInBlocks;
                    }
                }
                else
                {
                    xBlockItr = (blockX + x) % mapWidthInBlocks;
                    if (xBlockItr < 0)
                    {
                        xBlockItr += mapWidthInBlocks;
                    }
                }

                for (int y = -2; y <= 2; y++)
                {
                    int yBlockItr = 0;
                    if (blockY < wrapHeightInBlocks)
                    {
                        yBlockItr = (blockY + y) % wrapHeightInBlocks;
                        if (yBlockItr < 0)
                        {
                            yBlockItr += wrapHeightInBlocks;
                        }
                    }
                    else
                    {
                        yBlockItr = (blockY + y) % mapHeightInBlocks;
                        if (yBlockItr < 0)
                        {
                            yBlockItr += mapHeightInBlocks;
                        }
                    }

                    int blocknum = (xBlockItr * mapHeightInBlocks) + yBlockItr;

                    //CRC caching
                    ushort crc = CRC.MapCRCs[mapID][blocknum];

                    byte[] landData = new byte[0];
                    byte[] staticsData = new byte[0];
                    Point2D blockPosition = new Point2D(xBlockItr, yBlockItr);
                    if (crc == ushort.MaxValue)
                    {
                        crc = GetBlockCrc(blockPosition, mapID, ref landData, ref staticsData);
                        CRC.MapCRCs[mapID][blocknum] = crc;
                    }

                    //Console.WriteLine(crc.ToString("X4") + " vs " + recievedCRCs[((x + 2) * 5) + y + 2].ToString("X4"));
                    //Console.WriteLine(String.Format("({0},{1})", blockPosition.X, blockPosition.Y));
                    if (crc != recievedCRCs[((x + 2) * 5) + (y + 2)])
                    {
                        if (landData.Length < 1)
                        {
                            from.Send(new UpdateTerrainPacket(blockPosition, from));
                            from.Send(new UpdateStaticsPacket(blockPosition, from));
                        }
                        else
                        {
                            from.Send(new UpdateTerrainPacket(landData, blocknum, from.Map.MapID));
                            from.Send(new UpdateStaticsPacket(staticsData, blocknum, from.Map.MapID));
                        }
                    }
                }
            }

            //if (refreshClientView)
            //{
            //  from.Send(new RefreshClientView());
            //}
        }
    }
}
