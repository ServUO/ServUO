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
using Server.Network;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace UltimaLive
{
    [Flags]
    public enum LocalUpdateFlags
    {
        None = 0,
        Terrain = 1,
        Statics = 2,
    }

    public interface IMapOperation
    {
        void DoOperation();
        void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain);
        int BlockNumber
        {
            get;
        }
        int MapNumber
        {
            get;
        }
        Point2D Location
        {
            get;
        }
    }

    public class MapOperationSeries : BaseMapOperation
    {
        private List<IMapOperation> m_Changes;

        public override int BlockNumber
        {
            get
            {
                return -1;
            }
        }

        public override int MapNumber
        {
            get
            {
                return m_MapNumber;
            }
        }

        public MapOperationSeries(IMapOperation startingChange, int mapNumber) : base(-1, -1, mapNumber)
        {
            m_Changes = new List<IMapOperation>
            {
                startingChange
            };
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            bool sendoutUpdates = false;

            if (blockUpdateChain == null)
            {
                blockUpdateChain = new Dictionary<int, LocalUpdateFlags>();
                sendoutUpdates = true;
            }

            foreach (IMapOperation mc in m_Changes)
            {
                mc.DoOperation(blockUpdateChain);
            }

            if (sendoutUpdates)
            {
                foreach (KeyValuePair<int, LocalUpdateFlags> kvp in blockUpdateChain)
                {
                    if (kvp.Key >= 0)
                    {
                        BaseMapOperation.SendOutLocalUpdates(m_MapNumber, kvp.Key, kvp.Value);
                    }
                }
            }
        }

        public void Add(IMapOperation mc)
        {
            m_Changes.Add(mc);
        }

        public void Remove(IMapOperation mc)
        {
            m_Changes.Remove(mc);
        }
    }

    public class BaseMapOperation : IMapOperation
    {
        #region Static Methods
        public static void SendOutLocalUpdates(int mapNum, int blockNumber, LocalUpdateFlags flags)
        {
            Map map = Map.Maps[mapNum];
            TileMatrix tm = map.Tiles;
            int x = ((blockNumber / tm.BlockHeight) * 8) + 4;
            int y = ((blockNumber % tm.BlockHeight) * 8) + 4;
            SendOutLocalUpdates(map, x, y, flags);
        }

        public static void SendOutLocalUpdates(Map map, int x, int y, LocalUpdateFlags flags)
        {
            List<Mobile> candidates = new List<Mobile>();

            IPooledEnumerable<Mobile> eable = map.GetMobilesInRange(new Point3D(x, y, 0));

            foreach (Mobile m in eable)
            {
                if (m.Player)
                {
                    candidates.Add(m);
                }
            }
            eable.Free();

            foreach (Mobile m in candidates)
            {
                if ((flags & LocalUpdateFlags.Terrain) == LocalUpdateFlags.Terrain)
                {
                    m.Send(new UpdateTerrainPacket(new Point2D(x >> 3, y >> 3), m));
                }

                if ((flags & LocalUpdateFlags.Statics) == LocalUpdateFlags.Statics)
                {
                    m.Send(new UpdateStaticsPacket(new Point2D(x >> 3, y >> 3), m));
                }
                //m.Send(new RefreshClientView());
            }
        }
        #endregion

        public bool ValidOperation { get; set; }
        protected int m_BlockNumber;
        protected int m_MapNumber;
        protected Point2D m_Location;
        protected TileMatrix m_Matrix;
        protected Map m_Map;

        #region IMapOperation
        public Point2D Location
        {
            get
            {
                return m_Location;
            }
        }

        public virtual int BlockNumber
        {
            get
            {
                return m_BlockNumber;
            }
        }

        public virtual int MapNumber
        {
            get
            {
                return m_MapNumber;
            }
        }

        public virtual void DoOperation()
        {
            DoOperation(null);
        }

        public virtual void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            List<int> associated;
            if (MapRegistry.MapAssociations.TryGetValue(m_MapNumber, out associated))
            {
                foreach (int mapnum in associated)
                {
                    Map map = Map.Maps[mapnum];
                    if (map != m_Map)
                    {
                        map.ChangeMatrix(m_Map.Tiles, false);
                    }

                    CRC.InvalidateBlockCRC(mapnum, m_BlockNumber);
                }
            }
        }
        #endregion

        public BaseMapOperation(int x, int y, int map)
        {
            m_MapNumber = map;
            if (map >= 0 && x >= 0 && y >= 0)
            {
                m_Map = Map.Maps[map];
                m_Matrix = m_Map.Tiles;
                m_BlockNumber = (((x >> 3) * m_Matrix.BlockHeight) + (y >> 3));
                m_Location = new Point2D(x, y);
            }
        }
    }

    #region Static Operations
    public class StaticOperation : BaseMapOperation
    {
        protected StaticTile[][][] m_Block;
        protected StaticTile[] m_StaticTiles;

        public StaticOperation(int x, int y, int mapNum) : base(x, y, mapNum)
        {
            ValidOperation = (m_Map != null && m_Matrix != null && x <= m_Map.Width && y <= m_Map.Height);
            if (!ValidOperation)
            {
                return;
            }

            m_StaticTiles = m_Matrix.GetStaticTiles(x, y);
            m_Block = m_Matrix.GetStaticBlock(x >> 3, y >> 3);

            // If the block we retrieved is the "m_EmptyStaticBlock" from the 
            // TileMatrix, then we need to create a new blank block and change 
            // our m_Block to reference the new blank block.
            if (m_Block == m_Matrix.GetStaticBlock(-1, -1))
            {
                StaticTile[][][] tempBlock = new StaticTile[8][][];

                for (int i = 0; i < 8; ++i)
                {
                    tempBlock[i] = new StaticTile[8][];

                    for (int j = 0; j < 8; ++j)
                    {
                        tempBlock[i][j] = new StaticTile[0];
                    }
                }
                //public void SetStaticBlock( int x, int y, StaticTile[][][] value )
                m_Matrix.SetStaticBlock(x >> 3, y >> 3, tempBlock);
                m_Block = m_Matrix.GetStaticBlock(x >> 3, y >> 3);
            }
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (!ValidOperation)
            {
                return;
            }
            //invalidate CRC
            base.DoOperation(blockUpdateChain);

            //mark block for saving statics
            MapChangeTracker.MarkStaticsBlockForSave(m_Map.MapID, new Point2D(m_Location.X >> 3, m_Location.Y >> 3));

            if (blockUpdateChain == null)
            {
                SendOutLocalUpdates(m_Map, m_Location.X, m_Location.Y, LocalUpdateFlags.Statics);
            }
            else
            {
                if (blockUpdateChain.ContainsKey(m_BlockNumber))
                {
                    blockUpdateChain[m_BlockNumber] = blockUpdateChain[m_BlockNumber] | LocalUpdateFlags.Statics;
                }
                else
                {
                    blockUpdateChain.Add(m_BlockNumber, LocalUpdateFlags.Statics);
                }
            }
        }
    }

    public class ExistingStaticOperation : StaticOperation
    {
        protected ushort m_OldID;
        protected sbyte m_OldZ;
        protected ushort m_OldHue;
        protected int m_TileIndex;
        protected StaticTarget m_StaticTarget;

        public ExistingStaticOperation(int mapNum, StaticTarget targ)
            : base(targ.X, targ.Y, mapNum)
        {
            try
            {
                m_StaticTarget = targ;
                int x = targ.X;
                int y = targ.Y;
                int z = targ.Z - TileData.ItemTable[targ.ItemID].CalcHeight;

                m_TileIndex = -1;
                for (int i = 0; i < m_StaticTiles.Length; i++)
                {
                    if (m_StaticTiles[i].Z == z && m_StaticTiles[i].ID == targ.ItemID)
                    {
                        m_TileIndex = i;
                    }
                }

                if (m_TileIndex >= 0)
                {
                    m_OldID = (ushort)m_StaticTiles[m_TileIndex].ID;
                    m_OldZ = (sbyte)m_StaticTiles[m_TileIndex].Z;
                    m_OldHue = (ushort)m_StaticTiles[m_TileIndex].Hue;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " " + e.StackTrace);
            }
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                base.DoOperation(blockUpdateChain);
            }
        }
    }

    public class AddStatic : StaticOperation
    {
        protected int m_NewID;
        protected int m_NewAltitude;
        protected int m_Hue;

        public AddStatic(int mapNumber, int newID, int newAltitude, int x, int y, int hue) : base(x, y, mapNumber)
        {
            m_NewID = newID;
            m_NewAltitude = newAltitude;
            m_Hue = hue;
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                StaticTile[] newTileList = new StaticTile[m_StaticTiles.Length + 1];
                Array.Copy(m_StaticTiles, newTileList, m_StaticTiles.Length);
                newTileList[newTileList.Length - 1] = new StaticTile((ushort)m_NewID, (sbyte)m_NewAltitude)
                {
                    Hue = m_Hue
                };
                m_Block[Location.X & 0x7][Location.Y & 0x7] = newTileList;
                base.DoOperation(blockUpdateChain);
            }
        }
    }

    public class IncStaticAltitude : ExistingStaticOperation
    {
        protected int m_Change;
        public IncStaticAltitude(int mapNum, StaticTarget targ, int altitudeChange)
            : base(mapNum, targ)
        {
            m_Change = altitudeChange;
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                if (m_StaticTiles == null || m_StaticTiles.Length == 0 || m_TileIndex >= m_StaticTiles.Length || m_TileIndex < 0)
                {
                    return;
                }

                m_StaticTiles[m_TileIndex].Set(m_OldID, (sbyte)(m_Change + m_OldZ));
                base.DoOperation(blockUpdateChain);
            }
        }
    }

    public class SetStaticAltitude : ExistingStaticOperation
    {
        protected int m_NewAltitude;
        public SetStaticAltitude(int mapNum, StaticTarget targ, int newAltitude)
            : base(mapNum, targ)
        {
            m_NewAltitude = newAltitude;
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                if (m_StaticTiles == null || m_StaticTiles.Length == 0 || m_TileIndex >= m_StaticTiles.Length || m_TileIndex < 0)
                {
                    return;
                }

                m_StaticTiles[m_TileIndex].Set(m_OldID, (sbyte)(m_NewAltitude - TileData.ItemTable[m_OldID].CalcHeight));
                base.DoOperation(blockUpdateChain);
            }
        }
    }

    public class SetStaticID : ExistingStaticOperation
    {
        protected int m_NewID;
        public SetStaticID(int mapNum, StaticTarget targ, int newID)
            : base(mapNum, targ)
        {
            m_NewID = newID;
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                if (m_StaticTiles == null || m_StaticTiles.Length == 0 || m_TileIndex >= m_StaticTiles.Length || m_TileIndex < 0)
                {
                    return;
                }

                m_StaticTiles[m_TileIndex].Set((ushort)m_NewID, m_OldZ);
                base.DoOperation(blockUpdateChain);
            }
        }
    }

    public class SetStaticHue : ExistingStaticOperation
    {
        protected int m_NewHue;
        public SetStaticHue(int mapNum, StaticTarget targ, int newHue) : base(mapNum, targ)
        {
            m_NewHue = newHue;
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                if (m_StaticTiles == null || m_StaticTiles.Length == 0 || m_TileIndex >= m_StaticTiles.Length || m_TileIndex < 0)
                {
                    return;
                }

                m_StaticTiles[m_TileIndex].SetHue(m_OldID, (short)m_NewHue);
                base.DoOperation(blockUpdateChain);
            }
        }
    }

    public class MoveStatic : ExistingStaticOperation
    {
        protected int m_destinationX;
        protected int m_destinationY;

        public MoveStatic(int mapNum, StaticTarget targetOfStaticToMove, int destinationX, int destinationY) : base(mapNum, targetOfStaticToMove)
        {
            m_destinationX = destinationX;
            m_destinationY = destinationY;
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                AddStatic addStatic = new AddStatic(m_MapNumber, m_OldID, m_OldZ, m_destinationX, m_destinationY, m_OldHue);
                DeleteStatic delStatic = new DeleteStatic(m_MapNumber, m_StaticTarget);
                MapOperationSeries moveSeries = new MapOperationSeries(addStatic, m_MapNumber);
                moveSeries.Add(delStatic);
                moveSeries.DoOperation(blockUpdateChain);
            }
        }
    }
    /*
        public class StaticPriority : ExistingStaticOperation
        {
          protected Map m_OpMap;
          protected bool m_Increment;

          public StaticPriority(int mapNum, StaticTarget targetOfStaticToMove, bool increment)
            : base(mapNum, targetOfStaticToMove)
          {
            m_OpMap = Map.Maps[mapNum];
            m_Increment = increment;
          }

          public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
          {
            int newpos=0;
            int curpos=0;
            StaticTile[] tiles = m_OpMap.Tiles.GetStaticTiles(m_StaticTarget.X, m_StaticTarget.Y, false);
            List<StaticTarget> allowed = new List<StaticTarget>();
            for(int i=0; i<tiles.Length; i++)
            {
                if(tiles[i].Z==m_StaticTarget.TrueZ)
                {
                    curpos++;
                    bool self=(tiles[i].ID == m_StaticTarget.ItemID && m_StaticTarget.Hue == tiles[i].Hue);
                    if(m_Increment)
                    {
                        if(self)
                        {
                            newpos=curpos;
                        }
                    }
                    else
                    {
                        if(self)
                        {
                            if(curpos==1)
                                return;//already in the bottom
                            else
                                newpos=curpos-2;
                        }
                    }
                    if(!self)
                    {
                        StaticTarget targ = new StaticTarget(m_OpMap, new Point3D(tiles[i].X, tiles[i].Y, tiles[i].Z), tiles[i].ID);
                        allowed.Add(targ);
                    }
                }
            }
            if(curpos==allowed.Count)
                return;//no need to increment, already at top
            MapOperationSeries moveSeries = null;
            for(int i=0; i<allowed.Count; i++)
            {
                AddStatic addStatic;
                DeleteStatic delStatic;
                if(!m_Increment && i==newpos)
                {
                    addStatic = new AddStatic(m_MapNumber, m_StaticTarget.ItemID, m_StaticTarget.TrueZ, m_StaticTarget.X, m_StaticTarget.Y, m_StaticTarget.Hue);
                    delStatic = new DeleteStatic(m_MapNumber, m_StaticTarget);
                    if(moveSeries==null)
                        moveSeries = new MapOperationSeries(delStatic, m_MapNumber);
                    else
                        moveSeries.Add(delStatic);
                    moveSeries.Add(addStatic);
                }
                addStatic = new AddStatic(m_MapNumber, allowed[i].ItemID, m_StaticTarget.TrueZ, m_StaticTarget.X, m_StaticTarget.Y, allowed[i].Hue);
                delStatic = new DeleteStatic(m_MapNumber, allowed[i]);
                if(moveSeries==null)
                    moveSeries = new MapOperationSeries(delStatic, m_MapNumber);
                else
                    moveSeries.Add(delStatic);
                moveSeries.Add(addStatic);
                if(m_Increment && i==newpos)
                {
                    addStatic = new AddStatic(m_MapNumber, m_StaticTarget.ItemID, m_StaticTarget.TrueZ, m_StaticTarget.X, m_StaticTarget.Y, m_StaticTarget.Hue);
                    delStatic = new DeleteStatic(m_MapNumber, m_StaticTarget);
                    if(moveSeries==null)
                        moveSeries = new MapOperationSeries(delStatic, m_MapNumber);
                    else
                        moveSeries.Add(delStatic);
                    moveSeries.Add(addStatic);
                }
            }
            if(moveSeries!=null)
                moveSeries.DoOperation(blockUpdateChain);
          }
        }
    */
    public class DeleteStatic : ExistingStaticOperation
    {
        public DeleteStatic(int mapNum, StaticTarget targ)
            : base(mapNum, targ)
        {
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                if (m_StaticTiles == null || m_StaticTiles.Length == 0 || m_TileIndex < 0 || m_TileIndex >= m_StaticTiles.Length)
                {
                    return;
                }

                StaticTile[] newTileList = new StaticTile[m_StaticTiles.Length - 1];
                //reconstruct the array, but leave out the deleted static
                int index = 0;
                for (int i = 0; i < m_StaticTiles.Length; i++)
                {
                    if (i != m_TileIndex)
                    {
                        newTileList[index] = new StaticTile((ushort)m_StaticTiles[i].ID, (byte)m_StaticTiles[i].X, (byte)m_StaticTiles[i].Y, (sbyte)m_StaticTiles[i].Z, (short)m_StaticTiles[i].Hue);
                        index++;
                    }
                }
                //reassign the array
                m_Block[Location.X & 0x7][Location.Y & 0x7] = newTileList;
                base.DoOperation(blockUpdateChain);
            }
        }
    }
    #endregion

    #region Land Operations
    public class LandOperation : BaseMapOperation
    {
        protected LandTile[] m_Block;
        protected short m_OldID;
        protected sbyte m_OldZ;
        protected int m_TileIndex;
        public LandOperation(int x, int y, int mapNum)
            : base(x, y, mapNum)
        {
            ValidOperation = (m_Map != null && m_Matrix != null && x <= m_Map.Width && y <= m_Map.Height);
            if (!ValidOperation)
            {
                return;
            }

            m_Block = m_Matrix.GetLandBlock(x >> 3, y >> 3);
            m_OldID = (short)m_Block[((y & 0x7) << 3) + (x & 0x7)].ID;
            m_OldZ = (sbyte)m_Block[((y & 0x7) << 3) + (x & 0x7)].Z;
            m_TileIndex = ((m_Location.Y & 0x7) << 3) + (m_Location.X & 0x7);
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                base.DoOperation(blockUpdateChain);
                MapChangeTracker.MarkLandBlockForSave(m_Map.MapID, new Point2D(m_Location.X >> 3, m_Location.Y >> 3));
                if (blockUpdateChain == null)
                {
                    SendOutLocalUpdates(m_Map, m_Location.X, m_Location.Y, LocalUpdateFlags.Terrain);
                }
                else
                {
                    if (blockUpdateChain.ContainsKey(m_BlockNumber))
                    {
                        blockUpdateChain[m_BlockNumber] = blockUpdateChain[m_BlockNumber] | LocalUpdateFlags.Terrain;
                    }
                    else
                    {
                        blockUpdateChain.Add(m_BlockNumber, LocalUpdateFlags.Terrain);
                    }
                }
            }
        }
    }

    public class IncLandAltitude : LandOperation
    {
        protected int m_Change;
        public IncLandAltitude(int x, int y, int mapNum, int change)
            : base(x, y, mapNum)
        {
            m_Change = change;
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                m_Block[m_TileIndex].Set(m_OldID, (sbyte)(m_Change + m_OldZ));
                base.DoOperation(blockUpdateChain);
            }
        }
    }

    public class SetLandID : LandOperation
    {
        protected int m_NewID;
        public SetLandID(int x, int y, int mapNum, int newID)
            : base(x, y, mapNum)
        {
            m_NewID = newID;
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                m_Block[m_TileIndex].Set((short)m_NewID, m_OldZ);
                base.DoOperation(blockUpdateChain);
            }
        }
    }

    public class SetLandAltitude : LandOperation
    {
        protected int m_Altitude;
        public SetLandAltitude(int x, int y, int mapNum, int altitude)
            : base(x, y, mapNum)
        {
            m_Altitude = altitude;
        }

        public override void DoOperation(Dictionary<int, LocalUpdateFlags> blockUpdateChain)
        {
            if (ValidOperation)
            {
                m_Block[m_TileIndex].Set(m_OldID, (sbyte)m_Altitude);
                base.DoOperation(blockUpdateChain);
            }
        }
    }
    #endregion
}
