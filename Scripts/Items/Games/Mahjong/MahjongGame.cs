using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Gumps;
using Server.Multis;

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
            this.Weight = 5.0;

            this.BuildWalls();
            this.m_DealerIndicator = new MahjongDealerIndicator(this, new Point2D(300, 300), MahjongPieceDirection.Up, MahjongWind.North);
            this.m_WallBreakIndicator = new MahjongWallBreakIndicator(this, new Point2D(335, 335));
            this.m_Dices = new MahjongDices(this);
            this.m_Players = new MahjongPlayers(this, MaxPlayers, BaseScore);
            this.m_LastReset = DateTime.UtcNow;
            this.m_Level = SecureLevel.CoOwners;
        }

        public MahjongGame(Serial serial)
            : base(serial)
        {
        }

        public MahjongTile[] Tiles
        {
            get
            {
                return this.m_Tiles;
            }
        }
        public MahjongDealerIndicator DealerIndicator
        {
            get
            {
                return this.m_DealerIndicator;
            }
        }
        public MahjongWallBreakIndicator WallBreakIndicator
        {
            get
            {
                return this.m_WallBreakIndicator;
            }
        }
        public MahjongDices Dices
        {
            get
            {
                return this.m_Dices;
            }
        }
        public MahjongPlayers Players
        {
            get
            {
                return this.m_Players;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SecureLevel Level
        {
            get
            {
                return this.m_Level;
            }
            set
            {
                this.m_Level = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool ShowScores
        {
            get
            {
                return this.m_ShowScores;
            }
            set
            {
                if (this.m_ShowScores == value)
                    return;

                this.m_ShowScores = value;

                if (value)
                    this.m_Players.SendPlayersPacket(true, true);

                this.m_Players.SendGeneralPacket(true, true);

                this.m_Players.SendLocalizedMessage(value ? 1062777 : 1062778); // The dealer has enabled/disabled score display.
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool SpectatorVision
        {
            get
            {
                return this.m_SpectatorVision;
            }
            set
            {
                if (this.m_SpectatorVision == value)
                    return;

                this.m_SpectatorVision = value;

                if (this.m_Players.IsInGamePlayer(this.m_Players.DealerPosition))
                    this.m_Players.Dealer.Send(new MahjongGeneralInfo(this));

                this.m_Players.SendTilesPacket(false, true);

                this.m_Players.SendLocalizedMessage(value ? 1062715 : 1062716); // The dealer has enabled/disabled Spectator Vision.

                this.InvalidateProperties();
            }
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (this.m_SpectatorVision)
                list.Add(1062717); // Spectator Vision Enabled
            else
                list.Add(1062718); // Spectator Vision Disabled
        }

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            this.m_Players.CheckPlayers();

            if (from.Alive && this.IsAccessibleTo(from) && this.m_Players.GetInGameMobiles(true, false).Count == 0)
                list.Add(new ResetGameEntry(this));

            SetSecureLevelEntry.AddTo(from, this, list);
        }

        public override void OnDoubleClick(Mobile from)
        {
            this.m_Players.CheckPlayers();

            this.m_Players.Join(from);
        }

        public void ResetGame(Mobile from)
        {
            if (DateTime.UtcNow - this.m_LastReset < TimeSpan.FromSeconds(5.0))
                return;

            this.m_LastReset = DateTime.UtcNow;

            if (from != null)
                this.m_Players.SendLocalizedMessage(1062771, from.Name); // ~1_name~ has reset the game.

            this.m_Players.SendRelievePacket(true, true);

            this.BuildWalls();
            this.m_DealerIndicator = new MahjongDealerIndicator(this, new Point2D(300, 300), MahjongPieceDirection.Up, MahjongWind.North);
            this.m_WallBreakIndicator = new MahjongWallBreakIndicator(this, new Point2D(335, 335));
            this.m_Players = new MahjongPlayers(this, MaxPlayers, BaseScore);
        }

        public void ResetWalls(Mobile from)
        {
            if (DateTime.UtcNow - this.m_LastReset < TimeSpan.FromSeconds(5.0))
                return;

            this.m_LastReset = DateTime.UtcNow;

            this.BuildWalls();

            this.m_Players.SendTilesPacket(true, true);

            if (from != null)
                this.m_Players.SendLocalizedMessage(1062696); // The dealer rebuilds the wall.
        }

        public int GetStackLevel(MahjongPieceDim dim)
        {
            int level = -1;
            foreach (MahjongTile tile in this.m_Tiles)
            {
                if (tile.StackLevel > level && dim.IsOverlapping(tile.Dimensions))
                    level = tile.StackLevel;
            }
            return level;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version

            writer.Write((int)this.m_Level);

            writer.Write(this.m_Tiles.Length);

            for (int i = 0; i < this.m_Tiles.Length; i++)
                this.m_Tiles[i].Save(writer);

            this.m_DealerIndicator.Save(writer);

            this.m_WallBreakIndicator.Save(writer);

            this.m_Dices.Save(writer);

            this.m_Players.Save(writer);

            writer.Write(this.m_ShowScores);
            writer.Write(this.m_SpectatorVision);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                    {
                        this.m_Level = (SecureLevel)reader.ReadInt();

                        goto case 0;
                    }
                case 0:
                    {
                        if (version < 1)
                            this.m_Level = SecureLevel.CoOwners;

                        int length = reader.ReadInt();
                        this.m_Tiles = new MahjongTile[length];

                        for (int i = 0; i < length; i++)
                            this.m_Tiles[i] = new MahjongTile(this, reader);

                        this.m_DealerIndicator = new MahjongDealerIndicator(this, reader);

                        this.m_WallBreakIndicator = new MahjongWallBreakIndicator(this, reader);

                        this.m_Dices = new MahjongDices(this, reader);

                        this.m_Players = new MahjongPlayers(this, reader);

                        this.m_ShowScores = reader.ReadBool();
                        this.m_SpectatorVision = reader.ReadBool();

                        this.m_LastReset = DateTime.UtcNow;

                        break;
                    }
            }
        }

        private void BuildHorizontalWall(ref int index, int x, int y, int stackLevel, MahjongPieceDirection direction, MahjongTileTypeGenerator typeGenerator)
        {
            for (int i = 0; i < 17; i++)
            {
                Point2D position = new Point2D(x + i * 20, y);
                this.m_Tiles[index + i] = new MahjongTile(this, index + i, typeGenerator.Next(), position, stackLevel, direction, false);
            }

            index += 17;
        }

        private void BuildVerticalWall(ref int index, int x, int y, int stackLevel, MahjongPieceDirection direction, MahjongTileTypeGenerator typeGenerator)
        {
            for (int i = 0; i < 17; i++)
            {
                Point2D position = new Point2D(x, y + i * 20);
                this.m_Tiles[index + i] = new MahjongTile(this, index + i, typeGenerator.Next(), position, stackLevel, direction, false);
            }

            index += 17;
        }

        private void BuildWalls()
        {
            this.m_Tiles = new MahjongTile[17 * 8];

            MahjongTileTypeGenerator typeGenerator = new MahjongTileTypeGenerator(4);

            int i = 0;

            this.BuildHorizontalWall(ref i, 165, 110, 0, MahjongPieceDirection.Up, typeGenerator);
            this.BuildHorizontalWall(ref i, 165, 115, 1, MahjongPieceDirection.Up, typeGenerator);

            this.BuildVerticalWall(ref i, 530, 165, 0, MahjongPieceDirection.Left, typeGenerator);
            this.BuildVerticalWall(ref i, 525, 165, 1, MahjongPieceDirection.Left, typeGenerator);

            this.BuildHorizontalWall(ref i, 165, 530, 0, MahjongPieceDirection.Down, typeGenerator);
            this.BuildHorizontalWall(ref i, 165, 525, 1, MahjongPieceDirection.Down, typeGenerator);

            this.BuildVerticalWall(ref i, 110, 165, 0, MahjongPieceDirection.Right, typeGenerator);
            this.BuildVerticalWall(ref i, 115, 165, 1, MahjongPieceDirection.Right, typeGenerator);
        }

        private class ResetGameEntry : ContextMenuEntry
        {
            private readonly MahjongGame m_Game;
            public ResetGameEntry(MahjongGame game)
                : base(6162)
            {
                this.m_Game = game;
            }

            public override void OnClick()
            {
                Mobile from = this.Owner.From;

                if (from.CheckAlive() && !this.m_Game.Deleted && this.m_Game.IsAccessibleTo(from) && this.m_Game.Players.GetInGameMobiles(true, false).Count == 0)
                    this.m_Game.ResetGame(from);
            }
        }
    }
}