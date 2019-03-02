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

using System.Collections.Generic;
using UltimaLive;

namespace Server.Network
{

    #region Update Statics Packet
    /* Update Statics Packet 
     * 
     * 
     * Original Packet: 
     * ------------------------------
     * Source: http://necrotoolz.sourceforge.net/kairpacketguide/packet3f.htm
     * 
     * 0x3f      Command 
     * ushort    Size of packet
     * uint      Block Number
     * uint      Number of statics
     * uint      Extra value for the index file
     * 
     * Static[number of statics]    7 bytes
     *       ushort      ArtID
     *       byte        X offset in the block
     *       byte        Y offset in the block
     *       sbyte       Z axis position of the static
     *       ushort      Hue Number
     *       
     * 
     * 
     * New Format:
     * ------------------------------
     * byte         0x3f
     * ushort       size of the packet
     * int          block_number
     * uint         number of statics
     * uint         extra value that we're splitting
     *              byte  hash
     *              byte  UltimaLive Command
	 *              ushort  mapnumber - if this is 0xFF it means its a query
	 * struct
	 * [number of statics]
	 *              ushort      ItemId      
	 *              byte        X           // Not actually stored in runuo
	 *              byte        Y           // Not actually stored in runuo
	 *              sbyte       Z           
	 *              ushort      Hue         //no longer used
	 *              
	 * 
	 * We're going to use this as a dual purpose packet. The extra field 
	 * will tell us if the packet should actually be used as a hash. The
	 * extra field is split into two padding bytes (0x00), 
	 * and an unsigned short that we're using as our map number.
	 * 
	 * If unsigned short representing the map has a maxvalue for a ushort 
	 * (0xffff), then we'll know its a packet that we're using to request 
	 * a set of 25 CRCs from the client.
	/**/
    public class UpdateStaticsPacket : Packet
    {

        public UpdateStaticsPacket(Point2D blockCoords, Mobile m)
            : base(0x3F)
        {
            Map playerMap = m.Map;
            TileMatrix tm = playerMap.Tiles;

            int blockNum = ((blockCoords.X * tm.BlockHeight) + blockCoords.Y);
            byte[] staticsData = BlockUtility.GetRawStaticsData(blockCoords, playerMap.MapID);
            WriteDataToStream(staticsData, blockNum, playerMap.MapID);
        }

        public UpdateStaticsPacket(byte[] staticsData, int blockNumber, int mapID)
            : base(0x3F)
        {
            WriteDataToStream(staticsData, blockNumber, mapID);
        }


        public void WriteDataToStream(byte[] staticsData, int blockNumber, int mapID)
        {
            //byte 000         -  cmd
            EnsureCapacity(15 + staticsData.Length);         //byte 001 to 002  -  packet size
            m_Stream.Write((uint)blockNumber);                    //byte 003 to 006  -  block number
            m_Stream.Write(staticsData.Length / 7);          //byte 007 to 010  -  number of statics in packet
            m_Stream.Write((ushort)0x0000);                       //byte 011 to 012  -  UltimaLive sequence number
            m_Stream.Write((byte)0x00);                           //byte 013         -  UltimaLive command (0x00 is a statics update)
            m_Stream.Write((byte)mapID);                          //byte 014         -  UltimaLive mapnumber
            m_Stream.Write(staticsData, 0, staticsData.Length);   //byte 015 to ???  -  statics data

            //Console.WriteLine(string.Format("Sending statics data for block: {0}, Map: {1}", blockNum, playerMap.MapID));
            //BlockUtility.WriteStaticsDataToConsole(staticsData);
        }
    }
    #endregion

    #region Query Client Hash Packet
    //This is now using a CRC check instead of versions
    public sealed class QueryClientHash : Packet
    {
        /*private static Dictionary<Mobile, ushort> s_Sequences = new Dictionary<Mobile, ushort>();
		public static Dictionary<Mobile, ushort> Sequences{get{return s_Sequences;}}*/
        public QueryClientHash(Mobile m)
            : base(0x3F)
        {
            /*if(!s_Sequences.ContainsKey(m))
				s_Sequences[m]=(ushort)Utility.Random(ushort.MaxValue);*/
            Map playerMap = m.Map;
            TileMatrix tm = playerMap.Tiles;
            int blocknum = (((m.Location.X >> 3) * tm.BlockHeight) + (m.Location.Y >> 3));
            //Console.WriteLine(String.Format("Block Query Hash: {0}", blocknum));

            //byte 000         -  cmd
            EnsureCapacity(15);                    //byte 001 to 002  -  packet size
            m_Stream.Write((uint)blocknum);           //byte 003 to 006  -  central block number for the query (block that player is standing in)
            m_Stream.Write(0);                   //byte 007 to 010  -  number of statics in the packet (0 for a query)
            m_Stream.Write((ushort)Utility.Random(4));             //byte 011 to 012  -  UltimaLive sequence number
            m_Stream.Write((byte)0xFF);                 //byte 013         -  UltimaLive command (0xFF is a block Query)
            m_Stream.Write((byte)playerMap.MapID);      //byte 014         -  UltimaLive mapnumber
        }
    }
    #endregion

    #region Update Map Definitions
    //This is sent to the client so the client knows the dimensions of extra maps.
    public sealed class MapDefinitions : Packet
    {
        public MapDefinitions()
            : base(0x3F)
        {
            int maps = MapRegistry.MapAssociations.Count;
            int length = maps * 9; //12 * 9 = 108
            int count = length / 7; //108 / 7 = 15
            int padding = 0;
            if (length - (count * 7) > 0) //
            {
                count++;
                padding = (count * 7) - length;
            }
            //byte 000 to 015  -  The first 15 bytes of this packet are always the same
            //byte 000         -  cmd
            EnsureCapacity(15 + length + padding); //byte 001 to 002  -  packet size
            m_Stream.Write((uint)0x00);                 //byte 003 to 006  -  block number, doesn't really apply in this case
            m_Stream.Write(count);                 //byte 007 to 010  -  number of statics in the packet, in this case its calculated to 
                                                   //                    hold enough space for all the map definitions
            m_Stream.Write((ushort)0x00);               //byte 011 to 012  -  UltimaLive sequence number, doesn't apply in this case
            m_Stream.Write((byte)0x01);                 //byte 013         -  UltimaLive command (0x01 is Update Map Definitions)
            m_Stream.Write((byte)0x00);                 //byte 014         -  UltimaLive mapnumber, doesn't apply in this case
                                                        //byte 015 to end  -  Map Definitions

            for (int i = 0; i < maps; i++)
            {
                MapRegistry.MapDefinition md = MapRegistry.Definitions[i];
                m_Stream.Write((byte)md.FileIndex);                 //iteration byte 000         -  map file index number
                m_Stream.Write((ushort)md.Dimensions.X);            //iteration byte 001 to 002  -  map width
                m_Stream.Write((ushort)md.Dimensions.Y);            //iteration byte 003 to 004  -  map height
                m_Stream.Write((ushort)md.WrapAroundDimensions.X);  //iteration byte 005 to 006  -  wrap around dimension X
                m_Stream.Write((ushort)md.WrapAroundDimensions.Y);  //iteration byte 007 to 008  -  wrap around dimension Y
            }
            m_Stream.Fill();
        }
    }
    #endregion

    #region Update Terrain Packet
    /* Update Terrain Packet 
	 * Source: http://necrotoolz.sourceforge.net/kairpacketguide/packet40.htm
	 * 
	 * Format:
	 * 
	 * byte         0x40
	 * uint         block number
	 * 
	 * struct[64]   maptiles
	 *              ushort      Tile Number 
	 *              sbyte       Z    
	 * uint         map grid header -> we're splitting this into two ushorts
	 *      ushort  padding
	 *      ushort  mapnumber
	 *      
	 * 
	 * 
	/**/
    public sealed class UpdateTerrainPacket : Packet
    {
        public UpdateTerrainPacket(Point2D blockCoords, Mobile m)
            : base(0x40, 0xC9)
        {
            Map playerMap = m.Map;
            TileMatrix tm = playerMap.Tiles;
            int blockNumber = ((blockCoords.X * tm.BlockHeight) + blockCoords.Y);
            byte[] landData = BlockUtility.GetLandData(blockCoords, playerMap.MapID);
            WriteDataToStream(landData, blockNumber, playerMap.MapID);
        }

        public UpdateTerrainPacket(byte[] landData, int blockNumber, int mapID)
            : base(0x40, 0xC9)
        {
            WriteDataToStream(landData, blockNumber, mapID);
        }

        public void WriteDataToStream(byte[] landData, int blockNumber, int mapID)
        {
            //Console.WriteLine(string.Format("Packet Constructor land block coords ({0},{1})", blockCoords.X, blockCoords.Y));
            //byte 000              -  cmd
            m_Stream.Write(blockNumber);                     //byte 001 through 004  -  blocknum
            m_Stream.Write(landData, 0, landData.Length);      //byte 005 through 196  -  land data
            m_Stream.Write((byte)0x00);                        //byte 197              -  padding
            m_Stream.Write((byte)0x00);                        //byte 198              -  padding
            m_Stream.Write((byte)0x00);                        //byte 199              -  padding
            m_Stream.Write((byte)mapID);                       //byte 200              -  map number
                                                               //Console.WriteLine(string.Format("Sending land data for block: {0} Map: {1}", blocknum, playerMap.MapID));
                                                               //BlockUtility.WriteLandDataToConsole(landData);
        }
    }
    #endregion

    #region TargetObjectList Packets

    public struct TargetObject
    {
        public int ItemID;
        public int Hue;
        public int xOffset;
        public int yOffset;
        public int zOffset;
    }

    /* Target Object List Packet
	 * Thank you -hash- from RunUO.com for providing the definition for this
	 */
    public sealed class TargetObjectList : Packet
    {
        public TargetObjectList(List<TargetObject> targetObjects, Mobile m, bool allowGround)
            : base(0xB4)
        {
            byte allowGroundByte = 0;
            if (allowGround == true)
            {
                allowGroundByte = 0x1;
            }
            int packetSize = 16 + (targetObjects.Count * 10);

            //byte 000              -  cmd
            EnsureCapacity(packetSize);            //byte 001 through 002  -  packet size
            m_Stream.Write(allowGroundByte);            //byte 003              -  Allow Ground
            m_Stream.Write(m.Serial);              //byte 004 through 007  -  target serial
            m_Stream.Write((ushort)0);                  //byte 008 through 009  -  x
            m_Stream.Write((ushort)0);                  //byte 010 through 011  -  y
            m_Stream.Write((ushort)0);                  //byte 012 through 013  -  z
            m_Stream.Write((ushort)targetObjects.Count);//byte 014 through 015  -  Number of Entries
            foreach (TargetObject t in targetObjects)   //byte 016 through end     target object entries (10 bytes each)
            {
                m_Stream.Write((ushort)t.ItemID);       //entry byte 000 through 001  -  Number of Entries
                m_Stream.Write((ushort)t.Hue);          //entry byte 002 through 003  -  Number of Entries
                m_Stream.Write((ushort)t.xOffset);      //entry byte 004 through 005  -  Number of Entries
                m_Stream.Write((ushort)t.yOffset);      //entry byte 006 through 007  -  Number of Entries
                m_Stream.Write((ushort)t.zOffset);      //entry byte 008 through 009  -  Number of Entries
            }
        }
    }
    #endregion

    #region LoginComplete
    public sealed class LiveLoginComplete : Packet
    {
        public LiveLoginComplete()
            : base(0x3F)
        {
            //1 byte packet number (0x3F)
            //2 bytes size of packet (15)
            //4 byte block num (0x01)
            //4 byte statics count
            //4 byte extra
            //byte 000 to 015  -  The first 15 bytes of this packet are always the same
            //byte 000         -  cmd
            EnsureCapacity(43);                               //byte 001 to 002  -  packet size
            m_Stream.Write((uint)0x01);           //byte 003 to 006  -  block number, doesn't really apply in this case
            m_Stream.Write((uint)4);                             //byte 007 to 010  -  number of statics in the packet - 0 in this case
            m_Stream.Write((ushort)0x0000);         //byte 011 to 012  -  UltimaLive sequence number, doesn't apply in this case
            m_Stream.Write((byte)0x02);             //byte 013         -  UltimaLive command (0x02 is Login Confirmation)
            m_Stream.Write((byte)0x00);             //byte 014         -  UltimaLive mapnumber, doesn't apply in this case
            if (UltimaLiveSettings.ShardIdentifier.Length < 28)
            {
                m_Stream.WriteAsciiFixed(UltimaLiveSettings.ShardIdentifier, UltimaLiveSettings.ShardIdentifier.Length);                                       //byte 015 to 042  -  shard identifier
                int remainingLength = 28 - UltimaLiveSettings.ShardIdentifier.Length;
                for (int i = 0; i < remainingLength; ++i)
                {
                    m_Stream.Write((byte)0x00);
                }
            }
            else
            {
                m_Stream.WriteAsciiFixed(UltimaLiveSettings.ShardIdentifier.Substring(0, 28), 28);
            }
        }
    }
    #endregion

    #region Refresh Client View Packet
    //This is now using a CRC check instead of versions
    public sealed class RefreshClientView : Packet
    {
        public RefreshClientView()
            : base(0x3F)
        {
            //byte 000         -  cmd
            EnsureCapacity(15);                    //byte 001 to 002  -  packet size
            m_Stream.Write((uint)0);                  //byte 003 to 006  -  central block number for the query (block that player is standing in)
            m_Stream.Write(0);                   //byte 007 to 010  -  number of statics in the packet (0 for a query)
            m_Stream.Write((ushort)0x0000);             //byte 011 to 012  -  UltimaLive sequence number
            m_Stream.Write((byte)0x03);                 //byte 013         -  UltimaLive command (0x03 is a REFRESH_CLIENT)
            m_Stream.Write((byte)0);                    //byte 014         -  UltimaLive mapnumber
        }
    }
    #endregion
}