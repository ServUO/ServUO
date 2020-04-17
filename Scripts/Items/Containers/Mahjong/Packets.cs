using Server.Network;

namespace Server.Engines.Mahjong
{
    public sealed class MahjongJoinGame : Packet
    {
        public MahjongJoinGame(MahjongGame game)
            : base(0xDA)
        {
            EnsureCapacity(9);

            m_Stream.Write(game.Serial);
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)0x19);
        }
    }

    public sealed class MahjongPlayersInfo : Packet
    {
        public MahjongPlayersInfo(MahjongGame game, Mobile to)
            : base(0xDA)
        {
            MahjongPlayers players = game.Players;

            EnsureCapacity(11 + 45 * players.Seats);

            m_Stream.Write(game.Serial);
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)0x2);

            m_Stream.Write((byte)0);
            m_Stream.Write((byte)players.Seats);

            int n = 0;
            for (int i = 0; i < players.Seats; i++)
            {
                Mobile mobile = players.GetPlayer(i);

                if (mobile != null)
                {
                    m_Stream.Write(mobile.Serial);
                    m_Stream.Write(players.DealerPosition == i ? (byte)0x1 : (byte)0x2);
                    m_Stream.Write((byte)i);

                    if (game.ShowScores || mobile == to)
                        m_Stream.Write(players.GetScore(i));
                    else
                        m_Stream.Write(0);

                    m_Stream.Write((short)0);
                    m_Stream.Write((byte)0);

                    m_Stream.Write(players.IsPublic(i));

                    m_Stream.WriteAsciiFixed(mobile.Name, 30);
                    m_Stream.Write(!players.IsInGamePlayer(i));

                    n++;
                }
                else if (game.ShowScores)
                {
                    m_Stream.Write(0);
                    m_Stream.Write((byte)0x2);
                    m_Stream.Write((byte)i);

                    m_Stream.Write(players.GetScore(i));

                    m_Stream.Write((short)0);
                    m_Stream.Write((byte)0);

                    m_Stream.Write(players.IsPublic(i));

                    m_Stream.WriteAsciiFixed("", 30);
                    m_Stream.Write(true);

                    n++;
                }
            }

            if (n != players.Seats)
            {
                m_Stream.Seek(10, System.IO.SeekOrigin.Begin);
                m_Stream.Write((byte)n);
            }
        }
    }

    public sealed class MahjongGeneralInfo : Packet
    {
        public MahjongGeneralInfo(MahjongGame game)
            : base(0xDA)
        {
            EnsureCapacity(13);

            m_Stream.Write(game.Serial);
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)0x5);

            m_Stream.Write((short)0);
            m_Stream.Write((byte)0);

            m_Stream.Write((byte)((game.ShowScores ? 0x1 : 0x0) | (game.SpectatorVision ? 0x2 : 0x0)));

            m_Stream.Write((byte)game.Dices.First);
            m_Stream.Write((byte)game.Dices.Second);

            m_Stream.Write((byte)game.DealerIndicator.Wind);
            m_Stream.Write((short)game.DealerIndicator.Position.Y);
            m_Stream.Write((short)game.DealerIndicator.Position.X);
            m_Stream.Write((byte)game.DealerIndicator.Direction);

            m_Stream.Write((short)game.WallBreakIndicator.Position.Y);
            m_Stream.Write((short)game.WallBreakIndicator.Position.X);
        }
    }

    public sealed class MahjongTilesInfo : Packet
    {
        public MahjongTilesInfo(MahjongGame game, Mobile to)
            : base(0xDA)
        {
            MahjongTile[] tiles = game.Tiles;
            MahjongPlayers players = game.Players;

            EnsureCapacity(11 + 9 * tiles.Length);

            m_Stream.Write(game.Serial);
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)0x4);

            m_Stream.Write((short)tiles.Length);

            foreach (MahjongTile tile in tiles)
            {
                m_Stream.Write((byte)tile.Number);

                if (tile.Flipped)
                {
                    int hand = tile.Dimensions.GetHandArea();

                    if (hand < 0 || players.IsPublic(hand) || players.GetPlayer(hand) == to || (game.SpectatorVision && players.IsSpectator(to)))
                        m_Stream.Write((byte)tile.Value);
                    else
                        m_Stream.Write((byte)0);
                }
                else
                {
                    m_Stream.Write((byte)0);
                }

                m_Stream.Write((short)tile.Position.Y);
                m_Stream.Write((short)tile.Position.X);
                m_Stream.Write((byte)tile.StackLevel);
                m_Stream.Write((byte)tile.Direction);

                m_Stream.Write(tile.Flipped ? (byte)0x10 : (byte)0x0);
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

            EnsureCapacity(18);

            m_Stream.Write(tile.Game.Serial);
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)0x3);

            m_Stream.Write((byte)tile.Number);

            if (tile.Flipped)
            {
                int hand = tile.Dimensions.GetHandArea();

                if (hand < 0 || players.IsPublic(hand) || players.GetPlayer(hand) == to || (game.SpectatorVision && players.IsSpectator(to)))
                    m_Stream.Write((byte)tile.Value);
                else
                    m_Stream.Write((byte)0);
            }
            else
            {
                m_Stream.Write((byte)0);
            }

            m_Stream.Write((short)tile.Position.Y);
            m_Stream.Write((short)tile.Position.X);
            m_Stream.Write((byte)tile.StackLevel);
            m_Stream.Write((byte)tile.Direction);

            m_Stream.Write(tile.Flipped ? (byte)0x10 : (byte)0x0);
        }
    }

    public sealed class MahjongRelieve : Packet
    {
        public MahjongRelieve(MahjongGame game)
            : base(0xDA)
        {
            EnsureCapacity(9);

            m_Stream.Write(game.Serial);
            m_Stream.Write((byte)0);
            m_Stream.Write((byte)0x1A);
        }
    }
}