using System;
using Server.Network;

namespace Server.Engines.Mahjong
{
    public sealed class MahjongJoinGame : Packet
    {
        public MahjongJoinGame(MahjongGame game)
            : base(0xDA)
        {
            this.EnsureCapacity(9);

            this.m_Stream.Write((int)game.Serial);
            this.m_Stream.Write((byte)0);
            this.m_Stream.Write((byte)0x19);
        }
    }

    public sealed class MahjongPlayersInfo : Packet
    {
        public MahjongPlayersInfo(MahjongGame game, Mobile to)
            : base(0xDA)
        {
            MahjongPlayers players = game.Players;

            this.EnsureCapacity(11 + 45 * players.Seats);

            this.m_Stream.Write((int)game.Serial);
            this.m_Stream.Write((byte)0);
            this.m_Stream.Write((byte)0x2);

            this.m_Stream.Write((byte)0);
            this.m_Stream.Write((byte)players.Seats);

            int n = 0;
            for (int i = 0; i < players.Seats; i++)
            {
                Mobile mobile = players.GetPlayer(i);

                if (mobile != null)
                {
                    this.m_Stream.Write((int)mobile.Serial);
                    this.m_Stream.Write(players.DealerPosition == i ? (byte)0x1 : (byte)0x2);
                    this.m_Stream.Write((byte)i);

                    if (game.ShowScores || mobile == to)
                        this.m_Stream.Write((int)players.GetScore(i));
                    else
                        this.m_Stream.Write((int)0);

                    this.m_Stream.Write((short)0);
                    this.m_Stream.Write((byte)0);

                    this.m_Stream.Write(players.IsPublic(i));

                    this.m_Stream.WriteAsciiFixed(mobile.Name, 30);
                    this.m_Stream.Write(!players.IsInGamePlayer(i));

                    n++;
                }
                else if (game.ShowScores)
                {
                    this.m_Stream.Write((int)0);
                    this.m_Stream.Write((byte)0x2);
                    this.m_Stream.Write((byte)i);

                    this.m_Stream.Write((int)players.GetScore(i));

                    this.m_Stream.Write((short)0);
                    this.m_Stream.Write((byte)0);

                    this.m_Stream.Write(players.IsPublic(i));

                    this.m_Stream.WriteAsciiFixed("", 30);
                    this.m_Stream.Write(true);

                    n++;
                }
            }

            if (n != players.Seats)
            {
                this.m_Stream.Seek(10, System.IO.SeekOrigin.Begin);
                this.m_Stream.Write((byte)n);
            }
        }
    }

    public sealed class MahjongGeneralInfo : Packet
    {
        public MahjongGeneralInfo(MahjongGame game)
            : base(0xDA)
        {
            this.EnsureCapacity(13);

            this.m_Stream.Write((int)game.Serial);
            this.m_Stream.Write((byte)0);
            this.m_Stream.Write((byte)0x5);

            this.m_Stream.Write((short)0);
            this.m_Stream.Write((byte)0);

            this.m_Stream.Write((byte)((game.ShowScores ? 0x1 : 0x0) | (game.SpectatorVision ? 0x2 : 0x0)));

            this.m_Stream.Write((byte)game.Dices.First);
            this.m_Stream.Write((byte)game.Dices.Second);

            this.m_Stream.Write((byte)game.DealerIndicator.Wind);
            this.m_Stream.Write((short)game.DealerIndicator.Position.Y);
            this.m_Stream.Write((short)game.DealerIndicator.Position.X);
            this.m_Stream.Write((byte)game.DealerIndicator.Direction);

            this.m_Stream.Write((short)game.WallBreakIndicator.Position.Y);
            this.m_Stream.Write((short)game.WallBreakIndicator.Position.X);
        }
    }

    public sealed class MahjongTilesInfo : Packet
    {
        public MahjongTilesInfo(MahjongGame game, Mobile to)
            : base(0xDA)
        {
            MahjongTile[] tiles = game.Tiles;
            MahjongPlayers players = game.Players;

            this.EnsureCapacity(11 + 9 * tiles.Length);

            this.m_Stream.Write((int)game.Serial);
            this.m_Stream.Write((byte)0);
            this.m_Stream.Write((byte)0x4);

            this.m_Stream.Write((short)tiles.Length);

            foreach (MahjongTile tile in tiles)
            {
                this.m_Stream.Write((byte)tile.Number);

                if (tile.Flipped)
                {
                    int hand = tile.Dimensions.GetHandArea();

                    if (hand < 0 || players.IsPublic(hand) || players.GetPlayer(hand) == to || (game.SpectatorVision && players.IsSpectator(to)))
                        this.m_Stream.Write((byte)tile.Value);
                    else
                        this.m_Stream.Write((byte)0);
                }
                else
                {
                    this.m_Stream.Write((byte)0);
                }

                this.m_Stream.Write((short)tile.Position.Y);
                this.m_Stream.Write((short)tile.Position.X);
                this.m_Stream.Write((byte)tile.StackLevel);
                this.m_Stream.Write((byte)tile.Direction);

                this.m_Stream.Write(tile.Flipped ? (byte)0x10 : (byte)0x0);
            }
        }
    }

    public sealed class MahjongTileInfo : Packet
    {
        public MahjongTileInfo(MahjongTile tile, Mobile to)
            : base(0xDA)
        {
            MahjongGame game = tile.Game;
            MahjongPlayers players = game.Players;

            this.EnsureCapacity(18);

            this.m_Stream.Write((int)tile.Game.Serial);
            this.m_Stream.Write((byte)0);
            this.m_Stream.Write((byte)0x3);

            this.m_Stream.Write((byte)tile.Number);

            if (tile.Flipped)
            {
                int hand = tile.Dimensions.GetHandArea();

                if (hand < 0 || players.IsPublic(hand) || players.GetPlayer(hand) == to || (game.SpectatorVision && players.IsSpectator(to)))
                    this.m_Stream.Write((byte)tile.Value);
                else
                    this.m_Stream.Write((byte)0);
            }
            else
            {
                this.m_Stream.Write((byte)0);
            }

            this.m_Stream.Write((short)tile.Position.Y);
            this.m_Stream.Write((short)tile.Position.X);
            this.m_Stream.Write((byte)tile.StackLevel);
            this.m_Stream.Write((byte)tile.Direction);

            this.m_Stream.Write(tile.Flipped ? (byte)0x10 : (byte)0x0);
        }
    }

    public sealed class MahjongRelieve : Packet
    {
        public MahjongRelieve(MahjongGame game)
            : base(0xDA)
        {
            this.EnsureCapacity(9);

            this.m_Stream.Write((int)game.Serial);
            this.m_Stream.Write((byte)0);
            this.m_Stream.Write((byte)0x1A);
        }
    }
}