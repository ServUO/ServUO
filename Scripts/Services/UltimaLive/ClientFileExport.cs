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
using Server.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using Ultima;

namespace UltimaLive
{
    public class ClientFileExport
    {
        public static void Initialize()
        {
            Register("ExportClientFiles", AccessLevel.Owner, new CommandEventHandler(exportClientFiles_OnCommand));
            Register("PrintLandData", AccessLevel.Owner, new CommandEventHandler(printLandData_OnCommand));
            Register("PrintStaticsData", AccessLevel.Owner, new CommandEventHandler(printStaticsData_OnCommand));
            Register("PrintCrc", AccessLevel.Owner, new CommandEventHandler(printCrc_OnCommand));
        }

        [Usage("PrintCrc")]
        [Description("Prints to console the CRC data.")]
        private static void printCrc_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Server.Map playerMap = from.Map;
            Server.TileMatrix tm = playerMap.Tiles;

            int blocknum = (((from.Location.X >> 3) * tm.BlockHeight) + (from.Location.Y >> 3));
            byte[] LandData = BlockUtility.GetLandData(blocknum, playerMap.MapID);
            byte[] StaticsData = BlockUtility.GetRawStaticsData(blocknum, playerMap.MapID);

            byte[] blockData = new byte[LandData.Length + StaticsData.Length];
            Array.Copy(LandData, blockData, LandData.Length);
            Array.Copy(StaticsData, 0, blockData, LandData.Length, StaticsData.Length);


            ushort crc = CRC.Fletcher16(blockData);
            Console.WriteLine("CRC is 0x" + crc.ToString("X4"));

        }

        [Usage("PrintLandData")]
        [Description("Prints to console the land data.")]
        private static void printLandData_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Server.Map playerMap = from.Map;
            Server.TileMatrix tm = playerMap.Tiles;

            int blocknum = (((from.Location.X >> 3) * tm.BlockHeight) + (from.Location.Y >> 3));
            byte[] data = BlockUtility.GetLandData(blocknum, playerMap.MapID);
            BlockUtility.WriteLandDataToConsole(data);
        }

        [Usage("PrintStaticsData")]
        [Description("Prints to console the statics data.")]
        private static void printStaticsData_OnCommand(CommandEventArgs e)
        {
            Mobile from = e.Mobile;
            Server.Map playerMap = from.Map;
            Server.TileMatrix tm = playerMap.Tiles;

            int blocknum = (((from.Location.X >> 3) * tm.BlockHeight) + (from.Location.Y >> 3));
            byte[] data = BlockUtility.GetRawStaticsData(blocknum, playerMap.MapID);
            BlockUtility.WriteDataToConsole(data);

        }


        [Usage("ExportClientFiles")]
        [Description("Exports client file after save.")]
        private static void exportClientFiles_OnCommand(CommandEventArgs e)
        {
            ExportOnNextSave = true;
            Server.Misc.AutoSave.Save();
        }

        public static void Register(string command, AccessLevel access, CommandEventHandler handler)
        {
            CommandSystem.Register(command, access, handler);
        }

        public static void Configure()
        {
            EventSink.WorldSave += new WorldSaveEventHandler(OnSave);
        }

        private static bool ExportOnNextSave = false;

        //path used for hashes
        public const string ExportPathClientFies = @"ClientFilesExport";
        private static Server.Map m_WorkMap = null;
        public static Server.Map WorkMap
        {
            get
            { return m_WorkMap; }
        }
        public static void OnSave(WorldSaveEventArgs e)
        {
            if (!ExportOnNextSave)
            {
                return;
            }
            
            Files.SetMulPath(Core.DataDirectories[0]);
            Art.Reload();

            ExportOnNextSave = false;

            if (!Directory.Exists(ExportPathClientFies))
            {
                Directory.CreateDirectory(ExportPathClientFies);
            }

            Console.WriteLine("Exporting Client Files...");

            /* maps */
            // public static Dictionary<int, MapDefinition> Definitions
            int wrongln = 0;
            foreach (KeyValuePair<int, MapRegistry.MapDefinition> kvp in MapRegistry.Definitions)
            {
                if (!MapRegistry.MapAssociations.ContainsKey(kvp.Key))
                {
                    continue;
                }

                string filename = string.Format("map{0}.mul", kvp.Key);
                GenericWriter writer = new BinaryFileWriter(Path.Combine(ExportPathClientFies, filename), true);
                m_WorkMap = Server.Map.Maps[kvp.Key];
                Server.TileMatrix CurrentMatrix = m_WorkMap.Tiles;
                for (int xblock = 0; xblock < CurrentMatrix.BlockWidth; xblock++)
                {
                    for (int yblock = 0; yblock < CurrentMatrix.BlockHeight; yblock++)
                    {
                        writer.Write((uint)0);
                        LandTile[] blocktiles = CurrentMatrix.GetLandBlock(xblock, yblock);
                        if (blocktiles.Length == 196)
                        {
                            Console.WriteLine("Invalid landblock! Save failed!");
                            return;
                        }
                        else
                        {
                            for (int j = 0; j < 64; j++)
                            {
                                if (Art.IsValidLand(blocktiles[j].ID))
                                {
                                    writer.Write((short)blocktiles[j].ID);
                                    writer.Write((sbyte)blocktiles[j].Z);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid LAND at xyz {0} {1} {2} MAP {3} ID {4} - Replacing with NODRAW (land ID 0x0002)", ((xblock << 3) + (j & 0x7)), ((yblock << 3) + (j >> 3)), blocktiles[j].Z, m_WorkMap, blocktiles[j].ID);
                                    writer.Write((short)0x0002);
                                    writer.Write((sbyte)blocktiles[j].Z);
                                    wrongln++;
                                }
                            }
                        }
                    }
                }
                writer.Close();
            }

            /* Statics */
            int duplicated = 0;
            int wrongst = 0;
            foreach (KeyValuePair<int, MapRegistry.MapDefinition> kvp in MapRegistry.Definitions)
            {
                if (!MapRegistry.MapAssociations.ContainsKey(kvp.Key))
                {
                    continue;
                }

                string filename = string.Format("statics{0}.mul", kvp.Key);
                GenericWriter staticWriter = new BinaryFileWriter(Path.Combine(ExportPathClientFies, filename), true);
                filename = string.Format("staidx{0}.mul", kvp.Key);
                GenericWriter staticIndexWriter = new BinaryFileWriter(Path.Combine(ExportPathClientFies, filename), true);
                m_WorkMap = Server.Map.Maps[kvp.Key];
                Server.TileMatrix CurrentMatrix = m_WorkMap.Tiles;

                int startBlock = 0;
                int finishBlock = 0;

                for (int xblock = 0; xblock < CurrentMatrix.BlockWidth; xblock++)
                {
                    for (int yblock = 0; yblock < CurrentMatrix.BlockHeight; yblock++)
                    {
                        Server.StaticTile[][][] staticTiles = CurrentMatrix.GetStaticBlock(xblock, yblock);
                        //Static File
                        for (int i = 0; i < staticTiles.Length; i++)
                        {
                            for (int j = 0; j < staticTiles[i].Length; j++)
                            {
                                Server.StaticTile[] sortedTiles = staticTiles[i][j];
                                //Array.Sort(sortedTiles, BlockUtility.CompareStaticTiles);
                                Dictionary<int, List<int>> foundtiles = new Dictionary<int, List<int>>();

                                for (int k = 0; k < sortedTiles.Length; k++)
                                {
                                    List<int> tileIDs;
                                    if (!foundtiles.TryGetValue(sortedTiles[k].Z, out tileIDs))
                                    {
                                        foundtiles[sortedTiles[k].Z] = tileIDs = new List<int>();
                                    }
                                    if (!tileIDs.Contains(sortedTiles[k].ID))
                                    {
                                        if (sortedTiles[k].ID > 1 && Art.IsValidStatic(sortedTiles[k].ID))
                                        {
                                            staticWriter.Write((ushort)sortedTiles[k].ID);
                                            staticWriter.Write((byte)i);
                                            staticWriter.Write((byte)j);
                                            staticWriter.Write((sbyte)sortedTiles[k].Z);
                                            staticWriter.Write((short)sortedTiles[k].Hue);
                                            finishBlock += 7;
                                            tileIDs.Add(sortedTiles[k].ID);
                                        }
                                        else
                                        {
                                            Console.WriteLine("Invalid Static at xyz {0} {1} {2} MAP {3} ID {4} - NOT SAVED!", sortedTiles[k].X, sortedTiles[k].Y, sortedTiles[k].Z, m_WorkMap, sortedTiles[k].ID);
                                            wrongst++;
                                        }
                                    }
                                    else
                                    {
                                        duplicated++;
                                    }
                                }
                            }
                        }

                        //Index File
                        if (finishBlock != startBlock)
                        {
                            staticIndexWriter.Write(startBlock); //lookup
                            staticIndexWriter.Write(finishBlock - startBlock); //length
                            staticIndexWriter.Write(0); //extra
                            startBlock = finishBlock;
                        }
                        else
                        {
                            staticIndexWriter.Write(uint.MaxValue); //lookup
                            staticIndexWriter.Write(uint.MaxValue); //length
                            staticIndexWriter.Write(uint.MaxValue); //extra
                        }
                    }
                }
                staticWriter.Close();
                staticIndexWriter.Close();
            }
            Console.WriteLine("Found {0} duplicate statics and {1} statics with wrong ID - Removed! Found {2} WRONG Land ID, replaced with ID 0x0002 'NO DRAW'", duplicated, wrongst, wrongln);
        }
    }
}
