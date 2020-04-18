using System.Collections;

namespace Server.Engines.Mahjong
{
    public class MahjongPlayers
    {
        private readonly MahjongGame m_Game;
        private readonly Mobile[] m_Players;
        private readonly bool[] m_InGame;
        private readonly bool[] m_PublicHand;
        private readonly int[] m_Scores;
        private readonly ArrayList m_Spectators;
        private int m_DealerPosition;
        public MahjongPlayers(MahjongGame game, int maxPlayers, int baseScore)
        {
            m_Game = game;
            m_Spectators = new ArrayList();

            m_Players = new Mobile[maxPlayers];
            m_InGame = new bool[maxPlayers];
            m_PublicHand = new bool[maxPlayers];
            m_Scores = new int[maxPlayers];

            for (int i = 0; i < m_Scores.Length; i++)
                m_Scores[i] = baseScore;
        }

        public MahjongPlayers(MahjongGame game, GenericReader reader)
        {
            m_Game = game;
            m_Spectators = new ArrayList();

            int version = reader.ReadInt();

            int seats = reader.ReadInt();
            m_Players = new Mobile[seats];
            m_InGame = new bool[seats];
            m_PublicHand = new bool[seats];
            m_Scores = new int[seats];

            for (int i = 0; i < seats; i++)
            {
                m_Players[i] = reader.ReadMobile();
                m_PublicHand[i] = reader.ReadBool();
                m_Scores[i] = reader.ReadInt();
            }

            m_DealerPosition = reader.ReadInt();
        }

        public MahjongGame Game => m_Game;
        public int Seats => m_Players.Length;
        public Mobile Dealer => m_Players[m_DealerPosition];
        public int DealerPosition => m_DealerPosition;
        public Mobile GetPlayer(int index)
        {
            if (index < 0 || index >= m_Players.Length)
                return null;
            else
                return m_Players[index];
        }

        public int GetPlayerIndex(Mobile mobile)
        {
            for (int i = 0; i < m_Players.Length; i++)
            {
                if (m_Players[i] == mobile)
                    return i;
            }
            return -1;
        }

        public bool IsInGameDealer(Mobile mobile)
        {
            if (Dealer != mobile)
                return false;
            else
                return m_InGame[m_DealerPosition];
        }

        public bool IsInGamePlayer(int index)
        {
            if (index < 0 || index >= m_Players.Length || m_Players[index] == null)
                return false;
            else
                return m_InGame[index];
        }

        public bool IsInGamePlayer(Mobile mobile)
        {
            int index = GetPlayerIndex(mobile);

            return IsInGamePlayer(index);
        }

        public bool IsSpectator(Mobile mobile)
        {
            return m_Spectators.Contains(mobile);
        }

        public int GetScore(int index)
        {
            if (index < 0 || index >= m_Scores.Length)
                return 0;
            else
                return m_Scores[index];
        }

        public bool IsPublic(int index)
        {
            if (index < 0 || index >= m_PublicHand.Length)
                return false;
            else
                return m_PublicHand[index];
        }

        public void SetPublic(int index, bool value)
        {
            if (index < 0 || index >= m_PublicHand.Length || m_PublicHand[index] == value)
                return;

            m_PublicHand[index] = value;

            SendTilesPacket(true, !m_Game.SpectatorVision);

            if (IsInGamePlayer(index))
                m_Players[index].SendLocalizedMessage(value ? 1062775 : 1062776); // Your hand is [not] publicly viewable.
        }

        public ArrayList GetInGameMobiles(bool players, bool spectators)
        {
            ArrayList list = new ArrayList();

            if (players)
            {
                for (int i = 0; i < m_Players.Length; i++)
                {
                    if (IsInGamePlayer(i))
                        list.Add(m_Players[i]);
                }
            }

            if (spectators)
            {
                list.AddRange(m_Spectators);
            }

            return list;
        }

        public void CheckPlayers()
        {
            bool removed = false;

            for (int i = 0; i < m_Players.Length; i++)
            {
                Mobile player = m_Players[i];

                if (player != null)
                {
                    if (player.Deleted)
                    {
                        m_Players[i] = null;

                        SendPlayerExitMessage(player);
                        UpdateDealer(true);

                        removed = true;
                    }
                    else if (m_InGame[i])
                    {
                        if (player.NetState == null)
                        {
                            m_InGame[i] = false;

                            SendPlayerExitMessage(player);
                            UpdateDealer(true);

                            removed = true;
                        }
                        else if (!m_Game.IsAccessibleTo(player) || player.Map != m_Game.Map || !player.InRange(m_Game.GetWorldLocation(), 5))
                        {
                            m_InGame[i] = false;

                            player.Send(new MahjongRelieve(m_Game));

                            SendPlayerExitMessage(player);
                            UpdateDealer(true);

                            removed = true;
                        }
                    }
                }
            }

            for (int i = 0; i < m_Spectators.Count;)
            {
                Mobile mobile = (Mobile)m_Spectators[i];

                if (mobile.NetState == null || mobile.Deleted)
                {
                    m_Spectators.RemoveAt(i);
                }
                else if (!m_Game.IsAccessibleTo(mobile) || mobile.Map != m_Game.Map || !mobile.InRange(m_Game.GetWorldLocation(), 5))
                {
                    m_Spectators.RemoveAt(i);

                    mobile.Send(new MahjongRelieve(m_Game));
                }
                else
                {
                    i++;
                }
            }

            if (removed && !UpdateSpectators())
                SendPlayersPacket(true, true);
        }

        public void Join(Mobile mobile)
        {
            int index = GetPlayerIndex(mobile);

            if (index >= 0)
            {
                AddPlayer(mobile, index, true);
            }
            else
            {
                int nextSeat = GetNextSeat();

                if (nextSeat >= 0)
                {
                    AddPlayer(mobile, nextSeat, true);
                }
                else
                {
                    AddSpectator(mobile);
                }
            }
        }

        public void LeaveGame(Mobile player)
        {
            int index = GetPlayerIndex(player);
            if (index >= 0)
            {
                m_InGame[index] = false;

                SendPlayerExitMessage(player);
                UpdateDealer(true);

                SendPlayersPacket(true, true);
            }
            else
            {
                m_Spectators.Remove(player);
            }
        }

        public void ResetScores(int value)
        {
            for (int i = 0; i < m_Scores.Length; i++)
            {
                m_Scores[i] = value;
            }

            SendPlayersPacket(true, m_Game.ShowScores);

            SendLocalizedMessage(1062697); // The dealer redistributes the score sticks evenly.
        }

        public void TransferScore(Mobile from, int toPosition, int amount)
        {
            int fromPosition = GetPlayerIndex(from);
            Mobile to = GetPlayer(toPosition);

            if (fromPosition < 0 || to == null || m_Scores[fromPosition] < amount)
                return;

            m_Scores[fromPosition] -= amount;
            m_Scores[toPosition] += amount;

            if (m_Game.ShowScores)
            {
                SendPlayersPacket(true, true);
            }
            else
            {
                from.Send(new MahjongPlayersInfo(m_Game, from));
                to.Send(new MahjongPlayersInfo(m_Game, to));
            }

            SendLocalizedMessage(1062774, string.Format("{0}\t{1}\t{2}", from.Name, to.Name, amount)); // ~1_giver~ gives ~2_receiver~ ~3_number~ points.
        }

        public void OpenSeat(int index)
        {
            Mobile player = GetPlayer(index);
            if (player == null)
                return;

            if (m_InGame[index])
                player.Send(new MahjongRelieve(m_Game));

            m_Players[index] = null;

            SendLocalizedMessage(1062699, player.Name); // ~1_name~ is relieved from the game by the dealer.

            UpdateDealer(true);

            if (!UpdateSpectators())
                SendPlayersPacket(true, true);
        }

        public void AssignDealer(int index)
        {
            Mobile to = GetPlayer(index);

            if (to == null || !m_InGame[index])
                return;

            int oldDealer = m_DealerPosition;

            m_DealerPosition = index;

            if (IsInGamePlayer(oldDealer))
                m_Players[oldDealer].Send(new MahjongPlayersInfo(m_Game, m_Players[oldDealer]));

            to.Send(new MahjongPlayersInfo(m_Game, to));

            SendDealerChangedMessage();
        }

        public void SendPlayersPacket(bool players, bool spectators)
        {
            foreach (Mobile mobile in GetInGameMobiles(players, spectators))
            {
                mobile.Send(new MahjongPlayersInfo(m_Game, mobile));
            }
        }

        public void SendGeneralPacket(bool players, bool spectators)
        {
            ArrayList mobiles = GetInGameMobiles(players, spectators);

            if (mobiles.Count == 0)
                return;

            MahjongGeneralInfo generalInfo = new MahjongGeneralInfo(m_Game);

            generalInfo.Acquire();

            foreach (Mobile mobile in mobiles)
            {
                mobile.Send(generalInfo);
            }

            generalInfo.Release();
        }

        public void SendTilesPacket(bool players, bool spectators)
        {
            foreach (Mobile mobile in GetInGameMobiles(players, spectators))
            {
                mobile.Send(new MahjongTilesInfo(m_Game, mobile));
            }
        }

        public void SendTilePacket(MahjongTile tile, bool players, bool spectators)
        {
            foreach (Mobile mobile in GetInGameMobiles(players, spectators))
            {
                mobile.Send(new MahjongTileInfo(tile, mobile));
            }
        }

        public void SendRelievePacket(bool players, bool spectators)
        {
            ArrayList mobiles = GetInGameMobiles(players, spectators);

            if (mobiles.Count == 0)
                return;

            MahjongRelieve relieve = new MahjongRelieve(m_Game);

            relieve.Acquire();

            foreach (Mobile mobile in mobiles)
            {
                mobile.Send(relieve);
            }

            relieve.Release();
        }

        public void SendLocalizedMessage(int number)
        {
            foreach (Mobile mobile in GetInGameMobiles(true, true))
            {
                mobile.SendLocalizedMessage(number);
            }
        }

        public void SendLocalizedMessage(int number, string args)
        {
            foreach (Mobile mobile in GetInGameMobiles(true, true))
            {
                mobile.SendLocalizedMessage(number, args);
            }
        }

        public void Save(GenericWriter writer)
        {
            writer.Write(0); // version

            writer.Write(Seats);

            for (int i = 0; i < Seats; i++)
            {
                writer.Write(m_Players[i]);
                writer.Write(m_PublicHand[i]);
                writer.Write(m_Scores[i]);
            }

            writer.Write(m_DealerPosition);
        }

        private void UpdateDealer(bool message)
        {
            if (IsInGamePlayer(m_DealerPosition))
                return;

            for (int i = m_DealerPosition + 1; i < m_Players.Length; i++)
            {
                if (IsInGamePlayer(i))
                {
                    m_DealerPosition = i;

                    if (message)
                        SendDealerChangedMessage();

                    return;
                }
            }

            for (int i = 0; i < m_DealerPosition; i++)
            {
                if (IsInGamePlayer(i))
                {
                    m_DealerPosition = i;

                    if (message)
                        SendDealerChangedMessage();

                    return;
                }
            }
        }

        private int GetNextSeat()
        {
            for (int i = m_DealerPosition; i < m_Players.Length; i++)
            {
                if (m_Players[i] == null)
                    return i;
            }

            for (int i = 0; i < m_DealerPosition; i++)
            {
                if (m_Players[i] == null)
                    return i;
            }

            return -1;
        }

        private bool UpdateSpectators()
        {
            if (m_Spectators.Count == 0)
                return false;

            int nextSeat = GetNextSeat();

            if (nextSeat >= 0)
            {
                Mobile newPlayer = (Mobile)m_Spectators[0];

                m_Spectators.RemoveAt(0);

                AddPlayer(newPlayer, nextSeat, false);

                UpdateSpectators();

                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddPlayer(Mobile player, int index, bool sendJoinGame)
        {
            m_Players[index] = player;
            m_InGame[index] = true;

            UpdateDealer(false);

            if (sendJoinGame)
                player.Send(new MahjongJoinGame(m_Game));

            SendPlayersPacket(true, true);

            player.Send(new MahjongGeneralInfo(m_Game));
            player.Send(new MahjongTilesInfo(m_Game, player));

            if (m_DealerPosition == index)
                SendLocalizedMessage(1062773, player.Name); // ~1_name~ has entered the game as the dealer.
            else
                SendLocalizedMessage(1062772, player.Name); // ~1_name~ has entered the game as a player.
        }

        private void AddSpectator(Mobile mobile)
        {
            if (!IsSpectator(mobile))
            {
                m_Spectators.Add(mobile);
            }

            mobile.Send(new MahjongJoinGame(m_Game));
            mobile.Send(new MahjongPlayersInfo(m_Game, mobile));
            mobile.Send(new MahjongGeneralInfo(m_Game));
            mobile.Send(new MahjongTilesInfo(m_Game, mobile));
        }

        private void SendDealerChangedMessage()
        {
            if (Dealer != null)
                SendLocalizedMessage(1062698, Dealer.Name); // ~1_name~ is assigned the dealer.
        }

        private void SendPlayerExitMessage(Mobile who)
        {
            SendLocalizedMessage(1062762, who.Name); // ~1_name~ has left the game.
        }
    }
}