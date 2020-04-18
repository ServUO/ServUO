using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;
using System;
using System.Collections.Generic;

namespace Server.Engines.Mahjong
{
    public class MahjongGame : Item, ISecurable
    {
        public const int MaxPlayers = 4;
        public const int BaseScore = 30000;
        private MahjongTile[] m_Tiles;
        private MahjongDealerIndicator m_DealerIndicator;
        private MahjongWallBreakIndicator m_WallBreakIndicator;
        private MahjongDices m_Dices;
        private MahjongPlayers m_Players;
        private bool m_ShowScores;
        private bool m_SpectatorVision;
        private DateTime m_LastReset;
        private SecureLevel m_Level;
        [Constructable]
        public MahjongGame()
            : base(0xFAA)
        {
            Weight = 5.0;

            BuildWalls();
            m_DealerIndicator = new MahjongDealerIndicator(this, new Point2D(300, 300), MahjongPieceDirection.Up, MahjongWind.North);
            m_WallBreakIndicator = new MahjongWallBreakIndicator(this, new Point2D(335, 335));
            m_Dices = new MahjongDices(this);
            m_Players = new MahjongPlayers(this, MaxPlayers, BaseScore);
            m_LastReset = DateTime.UtcNow;
            m_Level = SecureLevel.CoOwners;
        }

        public MahjongGame(Serial serial)
            : base(serial)
        {
        }

        public MahjongTile[] Tiles => m_Tiles;
        public MahjongDealerIndicator DealerIndicator => m_DealerIndicator;
        public MahjongWallBreakIndicator WallBreakIndicator => m_WallBreakIndicator;
        public MahjongDices Dices => m_Dices;
        public MahjongPlayers Players => m_Players;
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return m_Level;
            }
            set
            {
                m_Level = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowScores
        {
            get
            {
                return m_ShowScores;
            }
            set
            {
                if (m_ShowScores == value)
                    return;

                m_ShowScores = value;

                if (value)
                    m_Players.SendPlayersPacket(true, true);

                m_Players.SendGeneralPacket(true, true);

                m_Players.SendLocalizedMessage(value ? 1062777 : 1062778); // The dealer has enabled/disabled score display.
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool SpectatorVision
        {
            get
            {
                return m_SpectatorVision;
            }
            set
            {
                if (m_SpectatorVision == value)
                    return;

                m_SpectatorVision = value;

                if (m_Players.IsInGamePlayer(m_Players.DealerPosition))
                    m_Players.Dealer.Send(new MahjongGeneralInfo(this));

                m_Players.SendTilesPacket(false, true);

                m_Players.SendLocalizedMessage(value ? 1062715 : 1062716); // The dealer has enabled/disabled Spectator Vision.

                InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_SpectatorVision)
                list.Add(1062717); // Spectator Vision Enabled
            else
                list.Add(1062718); // Spectator Vision Disabled
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            m_Players.CheckPlayers();

            if (from.Alive && IsAccessibleTo(from) && m_Players.GetInGameMobiles(true, false).Count == 0)
                list.Add(new ResetGameEntry(this));

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void OnDoubleClick(Mobile from)
        {
            m_Players.CheckPlayers();

            m_Players.Join(from);
        }

        public void ResetGame(Mobile from)
        {
            if (DateTime.UtcNow - m_LastReset < TimeSpan.FromSeconds(5.0))
                return;

            m_LastReset = DateTime.UtcNow;

            if (from != null)
                m_Players.SendLocalizedMessage(1062771, from.Name); // ~1_name~ has reset the game.

            m_Players.SendRelievePacket(true, true);

            BuildWalls();
            m_DealerIndicator = new MahjongDealerIndicator(this, new Point2D(300, 300), MahjongPieceDirection.Up, MahjongWind.North);
            m_WallBreakIndicator = new MahjongWallBreakIndicator(this, new Point2D(335, 335));
            m_Players = new MahjongPlayers(this, MaxPlayers, BaseScore);
        }

        public void ResetWalls(Mobile from)
        {
            if (DateTime.UtcNow - m_LastReset < TimeSpan.FromSeconds(5.0))
                return;

            m_LastReset = DateTime.UtcNow;

            BuildWalls();

            m_Players.SendTilesPacket(true, true);

            if (from != null)
                m_Players.SendLocalizedMessage(1062696); // The dealer rebuilds the wall.
        }

        public int GetStackLevel(MahjongPieceDim dim)
        {
            int level = -1;
            foreach (MahjongTile tile in m_Tiles)
            {
                if (tile.StackLevel > level && dim.IsOverlapping(tile.Dimensions))
                    level = tile.StackLevel;
            }
            return level;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1); // version

            writer.Write((int)m_Level);

            writer.Write(m_Tiles.Length);

            for (int i = 0; i < m_Tiles.Length; i++)
                m_Tiles[i].Save(writer);

            m_DealerIndicator.Save(writer);

            m_WallBreakIndicator.Save(writer);

            m_Dices.Save(writer);

            m_Players.Save(writer);

            writer.Write(m_ShowScores);
            writer.Write(m_SpectatorVision);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    {
                        m_Level = (SecureLevel)reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 1)
                            m_Level = SecureLevel.CoOwners;

                        int length = reader.ReadInt();
                        m_Tiles = new MahjongTile[length];

                        for (int i = 0; i < length; i++)
                            m_Tiles[i] = new MahjongTile(this, reader);

                        m_DealerIndicator = new MahjongDealerIndicator(this, reader);

                        m_WallBreakIndicator = new MahjongWallBreakIndicator(this, reader);

                        m_Dices = new MahjongDices(this, reader);

                        m_Players = new MahjongPlayers(this, reader);

                        m_ShowScores = reader.ReadBool();
                        m_SpectatorVision = reader.ReadBool();

                        m_LastReset = DateTime.UtcNow;

                        break;
                    }
            }
        }

        private void BuildHorizontalWall(ref int index, int x, int y, int stackLevel, MahjongPieceDirection direction, MahjongTileTypeGenerator typeGenerator)
        {
            for (int i = 0; i < 17; i++)
            {
                Point2D position = new Point2D(x + i * 20, y);
                m_Tiles[index + i] = new MahjongTile(this, index + i, typeGenerator.Next(), position, stackLevel, direction, false);
            }

            index += 17;
        }

        private void BuildVerticalWall(ref int index, int x, int y, int stackLevel, MahjongPieceDirection direction, MahjongTileTypeGenerator typeGenerator)
        {
            for (int i = 0; i < 17; i++)
            {
                Point2D position = new Point2D(x, y + i * 20);
                m_Tiles[index + i] = new MahjongTile(this, index + i, typeGenerator.Next(), position, stackLevel, direction, false);
            }

            index += 17;
        }

        private void BuildWalls()
        {
            m_Tiles = new MahjongTile[17 * 8];

            MahjongTileTypeGenerator typeGenerator = new MahjongTileTypeGenerator(4);

            int i = 0;

            BuildHorizontalWall(ref i, 165, 110, 0, MahjongPieceDirection.Up, typeGenerator);
            BuildHorizontalWall(ref i, 165, 115, 1, MahjongPieceDirection.Up, typeGenerator);

            BuildVerticalWall(ref i, 530, 165, 0, MahjongPieceDirection.Left, typeGenerator);
            BuildVerticalWall(ref i, 525, 165, 1, MahjongPieceDirection.Left, typeGenerator);

            BuildHorizontalWall(ref i, 165, 530, 0, MahjongPieceDirection.Down, typeGenerator);
            BuildHorizontalWall(ref i, 165, 525, 1, MahjongPieceDirection.Down, typeGenerator);

            BuildVerticalWall(ref i, 110, 165, 0, MahjongPieceDirection.Right, typeGenerator);
            BuildVerticalWall(ref i, 115, 165, 1, MahjongPieceDirection.Right, typeGenerator);
        }

        private class ResetGameEntry : ContextMenuEntry
        {
            private readonly MahjongGame m_Game;
            public ResetGameEntry(MahjongGame game)
                : base(6162)
            {
                m_Game = game;
            }

            public override void OnClick()
            {
                Mobile from = Owner.From;

                if (from.CheckAlive() && !m_Game.Deleted && m_Game.IsAccessibleTo(from) && m_Game.Players.GetInGameMobiles(true, false).Count == 0)
                    m_Game.ResetGame(from);
            }
        }
    }
}