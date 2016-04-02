using System;
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
            this.m_Game = game;
            this.m_Spectators = new ArrayList();

            this.m_Players = new Mobile[maxPlayers];
            this.m_InGame = new bool[maxPlayers];
            this.m_PublicHand = new bool[maxPlayers];
            this.m_Scores = new int[maxPlayers];

            for (int i = 0; i < this.m_Scores.Length; i++)
                this.m_Scores[i] = baseScore;
        }

        public MahjongPlayers(MahjongGame game, GenericReader reader)
        {
            this.m_Game = game;
            this.m_Spectators = new ArrayList();

            int version = reader.ReadInt();

            int seats = reader.ReadInt();
            this.m_Players = new Mobile[seats];
            this.m_InGame = new bool[seats];
            this.m_PublicHand = new bool[seats];
            this.m_Scores = new int[seats];

            for (int i = 0; i < seats; i++)
            {
                this.m_Players[i] = reader.ReadMobile();
                this.m_PublicHand[i] = reader.ReadBool();
                this.m_Scores[i] = reader.ReadInt();
            }

            this.m_DealerPosition = reader.ReadInt();
        }

        public MahjongGame Game
        {
            get
            {
                return this.m_Game;
            }
        }
        public int Seats
        {
            get
            {
                return this.m_Players.Length;
            }
        }
        public Mobile Dealer
        {
            get
            {
                return this.m_Players[this.m_DealerPosition];
            }
        }
        public int DealerPosition
        {
            get
            {
                return this.m_DealerPosition;
            }
        }
        public Mobile GetPlayer(int index)
        {
            if (index < 0 || index >= this.m_Players.Length)
                return null;
            else
                return this.m_Players[index];
        }

        public int GetPlayerIndex(Mobile mobile)
        {
            for (int i = 0; i < this.m_Players.Length; i++)
            {
                if (this.m_Players[i] == mobile)
                    return i;
            }
            return -1;
        }

        public bool IsInGameDealer(Mobile mobile)
        {
            if (this.Dealer != mobile)
                return false;
            else
                return this.m_InGame[this.m_DealerPosition];
        }

        public bool IsInGamePlayer(int index)
        {
            if (index < 0 || index >= this.m_Players.Length || this.m_Players[index] == null)
                return false;
            else
                return this.m_InGame[index];
        }

        public bool IsInGamePlayer(Mobile mobile)
        {
            int index = this.GetPlayerIndex(mobile);

            return this.IsInGamePlayer(index);
        }

        public bool IsSpectator(Mobile mobile)
        {
            return this.m_Spectators.Contains(mobile);
        }

        public int GetScore(int index)
        {
            if (index < 0 || index >= this.m_Scores.Length)
                return 0;
            else
                return this.m_Scores[index];
        }

        public bool IsPublic(int index)
        {
            if (index < 0 || index >= this.m_PublicHand.Length)
                return false;
            else
                return this.m_PublicHand[index];
        }

        public void SetPublic(int index, bool value)
        {
            if (index < 0 || index >= this.m_PublicHand.Length || this.m_PublicHand[index] == value)
                return;

            this.m_PublicHand[index] = value;

            this.SendTilesPacket(true, !this.m_Game.SpectatorVision);

            if (this.IsInGamePlayer(index))
                this.m_Players[index].SendLocalizedMessage(value ? 1062775 : 1062776); // Your hand is [not] publicly viewable.
        }

        public ArrayList GetInGameMobiles(bool players, bool spectators)
        {
            ArrayList list = new ArrayList();

            if (players)
            {
                for (int i = 0; i < this.m_Players.Length; i++)
                {
                    if (this.IsInGamePlayer(i))
                        list.Add(this.m_Players[i]);
                }
            }

            if (spectators)
            {
                list.AddRange(this.m_Spectators);
            }

            return list;
        }

        public void CheckPlayers()
        {
            bool removed = false;

            for (int i = 0; i < this.m_Players.Length; i++)
            {
                Mobile player = this.m_Players[i];

                if (player != null)
                {
                    if (player.Deleted)
                    {
                        this.m_Players[i] = null;

                        this.SendPlayerExitMessage(player);
                        this.UpdateDealer(true);

                        removed = true;
                    }
                    else if (this.m_InGame[i])
                    {
                        if (player.NetState == null)
                        {
                            this.m_InGame[i] = false;

                            this.SendPlayerExitMessage(player);
                            this.UpdateDealer(true);

                            removed = true;
                        }
                        else if (!this.m_Game.IsAccessibleTo(player) || player.Map != this.m_Game.Map || !player.InRange(this.m_Game.GetWorldLocation(), 5))
                        {
                            this.m_InGame[i] = false;

                            player.Send(new MahjongRelieve(this.m_Game));

                            this.SendPlayerExitMessage(player);
                            this.UpdateDealer(true);

                            removed = true;
                        }
                    }
                }
            }

            for (int i = 0; i < this.m_Spectators.Count;)
            {
                Mobile mobile = (Mobile)this.m_Spectators[i];

                if (mobile.NetState == null || mobile.Deleted)
                {
                    this.m_Spectators.RemoveAt(i);
                }
                else if (!this.m_Game.IsAccessibleTo(mobile) || mobile.Map != this.m_Game.Map || !mobile.InRange(this.m_Game.GetWorldLocation(), 5))
                {
                    this.m_Spectators.RemoveAt(i);

                    mobile.Send(new MahjongRelieve(this.m_Game));
                }
                else
                {
                    i++;
                }
            }

            if (removed && !this.UpdateSpectators())
                this.SendPlayersPacket(true, true);
        }

        public void Join(Mobile mobile)
        {
            int index = this.GetPlayerIndex(mobile);

            if (index >= 0)
            {
                this.AddPlayer(mobile, index, true);
            }
            else
            {
                int nextSeat = this.GetNextSeat();

                if (nextSeat >= 0)
                {
                    this.AddPlayer(mobile, nextSeat, true);
                }
                else
                {
                    this.AddSpectator(mobile);
                }
            }
        }

        public void LeaveGame(Mobile player)
        {
            int index = this.GetPlayerIndex(player);
            if (index >= 0)
            {
                this.m_InGame[index] = false;

                this.SendPlayerExitMessage(player);
                this.UpdateDealer(true);

                this.SendPlayersPacket(true, true);
            }
            else
            {
                this.m_Spectators.Remove(player);
            }
        }

        public void ResetScores(int value)
        {
            for (int i = 0; i < this.m_Scores.Length; i++)
            {
                this.m_Scores[i] = value;
            }

            this.SendPlayersPacket(true, this.m_Game.ShowScores);

            this.SendLocalizedMessage(1062697); // The dealer redistributes the score sticks evenly.
        }

        public void TransferScore(Mobile from, int toPosition, int amount)
        {
            int fromPosition = this.GetPlayerIndex(from);
            Mobile to = this.GetPlayer(toPosition);

            if (fromPosition < 0 || to == null || this.m_Scores[fromPosition] < amount)
                return;

            this.m_Scores[fromPosition] -= amount;
            this.m_Scores[toPosition] += amount;

            if (this.m_Game.ShowScores)
            {
                this.SendPlayersPacket(true, true);
            }
            else
            {
                from.Send(new MahjongPlayersInfo(this.m_Game, from));
                to.Send(new MahjongPlayersInfo(this.m_Game, to));
            }

            this.SendLocalizedMessage(1062774, string.Format("{0}\t{1}\t{2}", from.Name, to.Name, amount)); // ~1_giver~ gives ~2_receiver~ ~3_number~ points.
        }

        public void OpenSeat(int index)
        {
            Mobile player = this.GetPlayer(index);
            if (player == null)
                return;

            if (this.m_InGame[index])
                player.Send(new MahjongRelieve(this.m_Game));

            this.m_Players[index] = null;

            this.SendLocalizedMessage(1062699, player.Name); // ~1_name~ is relieved from the game by the dealer.

            this.UpdateDealer(true);

            if (!this.UpdateSpectators())
                this.SendPlayersPacket(true, true);
        }

        public void AssignDealer(int index)
        {
            Mobile to = this.GetPlayer(index);

            if (to == null || !this.m_InGame[index])
                return;

            int oldDealer = this.m_DealerPosition;

            this.m_DealerPosition = index;

            if (this.IsInGamePlayer(oldDealer))
                this.m_Players[oldDealer].Send(new MahjongPlayersInfo(this.m_Game, this.m_Players[oldDealer]));

            to.Send(new MahjongPlayersInfo(this.m_Game, to));

            this.SendDealerChangedMessage();
        }

        public void SendPlayersPacket(bool players, bool spectators)
        {
            foreach (Mobile mobile in this.GetInGameMobiles(players, spectators))
            {
                mobile.Send(new MahjongPlayersInfo(this.m_Game, mobile));
            }
        }

        public void SendGeneralPacket(bool players, bool spectators)
        {
            ArrayList mobiles = this.GetInGameMobiles(players, spectators);

            if (mobiles.Count == 0)
                return;

            MahjongGeneralInfo generalInfo = new MahjongGeneralInfo(this.m_Game);

            generalInfo.Acquire();

            foreach (Mobile mobile in mobiles)
            {
                mobile.Send(generalInfo);
            }

            generalInfo.Release();
        }

        public void SendTilesPacket(bool players, bool spectators)
        {
            foreach (Mobile mobile in this.GetInGameMobiles(players, spectators))
            {
                mobile.Send(new MahjongTilesInfo(this.m_Game, mobile));
            }
        }

        public void SendTilePacket(MahjongTile tile, bool players, bool spectators)
        {
            foreach (Mobile mobile in this.GetInGameMobiles(players, spectators))
            {
                mobile.Send(new MahjongTileInfo(tile, mobile));
            }
        }

        public void SendRelievePacket(bool players, bool spectators)
        {
            ArrayList mobiles = this.GetInGameMobiles(players, spectators);

            if (mobiles.Count == 0)
                return;

            MahjongRelieve relieve = new MahjongRelieve(this.m_Game);

            relieve.Acquire();

            foreach (Mobile mobile in mobiles)
            {
                mobile.Send(relieve);
            }

            relieve.Release();
        }

        public void SendLocalizedMessage(int number)
        {
            foreach (Mobile mobile in this.GetInGameMobiles(true, true))
            {
                mobile.SendLocalizedMessage(number);
            }
        }

        public void SendLocalizedMessage(int number, string args)
        {
            foreach (Mobile mobile in this.GetInGameMobiles(true, true))
            {
                mobile.SendLocalizedMessage(number, args);
            }
        }

        public void Save(GenericWriter writer)
        {
            writer.Write((int)0); // version

            writer.Write(this.Seats);

            for (int i = 0; i < this.Seats; i++)
            {
                writer.Write(this.m_Players[i]);
                writer.Write(this.m_PublicHand[i]);
                writer.Write(this.m_Scores[i]);
            }

            writer.Write(this.m_DealerPosition);
        }

        private void UpdateDealer(bool message)
        {
            if (this.IsInGamePlayer(this.m_DealerPosition))
                return;

            for (int i = this.m_DealerPosition + 1; i < this.m_Players.Length; i++)
            {
                if (this.IsInGamePlayer(i))
                {
                    this.m_DealerPosition = i;

                    if (message)
                        this.SendDealerChangedMessage();

                    return;
                }
            }

            for (int i = 0; i < this.m_DealerPosition; i++)
            {
                if (this.IsInGamePlayer(i))
                {
                    this.m_DealerPosition = i;

                    if (message)
                        this.SendDealerChangedMessage();

                    return;
                }
            }
        }

        private int GetNextSeat()
        {
            for (int i = this.m_DealerPosition; i < this.m_Players.Length; i++)
            {
                if (this.m_Players[i] == null)
                    return i;
            }

            for (int i = 0; i < this.m_DealerPosition; i++)
            {
                if (this.m_Players[i] == null)
                    return i;
            }

            return -1;
        }

        private bool UpdateSpectators()
        {
            if (this.m_Spectators.Count == 0)
                return false;

            int nextSeat = this.GetNextSeat();

            if (nextSeat >= 0)
            {
                Mobile newPlayer = (Mobile)this.m_Spectators[0];

                this.m_Spectators.RemoveAt(0);

                this.AddPlayer(newPlayer, nextSeat, false);

                this.UpdateSpectators();

                return true;
            }
            else
            {
                return false;
            }
        }

        private void AddPlayer(Mobile player, int index, bool sendJoinGame)
        {
            this.m_Players[index] = player;
            this.m_InGame[index] = true;

            this.UpdateDealer(false);

            if (sendJoinGame)
                player.Send(new MahjongJoinGame(this.m_Game));

            this.SendPlayersPacket(true, true);

            player.Send(new MahjongGeneralInfo(this.m_Game));
            player.Send(new MahjongTilesInfo(this.m_Game, player));

            if (this.m_DealerPosition == index)
                this.SendLocalizedMessage(1062773, player.Name); // ~1_name~ has entered the game as the dealer.
            else
                this.SendLocalizedMessage(1062772, player.Name); // ~1_name~ has entered the game as a player.
        }

        private void AddSpectator(Mobile mobile)
        {
            if (!this.IsSpectator(mobile))
            {
                this.m_Spectators.Add(mobile);
            }

            mobile.Send(new MahjongJoinGame(this.m_Game));
            mobile.Send(new MahjongPlayersInfo(this.m_Game, mobile));
            mobile.Send(new MahjongGeneralInfo(this.m_Game));
            mobile.Send(new MahjongTilesInfo(this.m_Game, mobile));
        }

        private void SendDealerChangedMessage()
        {
            if (this.Dealer != null)
                this.SendLocalizedMessage(1062698, this.Dealer.Name); // ~1_name~ is assigned the dealer.
        }

        private void SendPlayerExitMessage(Mobile who)
        {
            this.SendLocalizedMessage(1062762, who.Name); // ~1_name~ has left the game.
        }
    }
}