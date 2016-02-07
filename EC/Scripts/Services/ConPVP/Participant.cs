using System;
using System.Text;
using Server.Mobiles;

namespace Server.Engines.ConPVP
{
    public class Participant
    {
        private readonly DuelContext m_Context;
        private DuelPlayer[] m_Players;
        private TournyParticipant m_TournyPart;
        public Participant(DuelContext context, int count)
        {
            this.m_Context = context;
            //m_Stakes = new StakesContainer( context, this );
            this.Resize(count);
        }

        public int Count
        {
            get
            {
                return this.m_Players.Length;
            }
        }
        public DuelPlayer[] Players
        {
            get
            {
                return this.m_Players;
            }
        }
        public DuelContext Context
        {
            get
            {
                return this.m_Context;
            }
        }
        public TournyParticipant TournyPart
        {
            get
            {
                return this.m_TournyPart;
            }
            set
            {
                this.m_TournyPart = value;
            }
        }
        public int FilledSlots
        {
            get
            {
                int count = 0;

                for (int i = 0; i < this.m_Players.Length; ++i)
                {
                    if (this.m_Players[i] != null)
                        ++count;
                }

                return count;
            }
        }
        public bool HasOpenSlot
        {
            get
            {
                for (int i = 0; i < this.m_Players.Length; ++i)
                {
                    if (this.m_Players[i] == null)
                        return true;
                }

                return false;
            }
        }
        public bool Eliminated
        {
            get
            {
                for (int i = 0; i < this.m_Players.Length; ++i)
                {
                    if (this.m_Players[i] != null && !this.m_Players[i].Eliminated)
                        return false;
                }

                return true;
            }
        }
        public string NameList
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < this.m_Players.Length; ++i)
                {
                    if (this.m_Players[i] == null)
                        continue;

                    Mobile mob = this.m_Players[i].Mobile;

                    if (sb.Length > 0)
                        sb.Append(", ");

                    sb.Append(mob.Name);
                }

                if (sb.Length == 0)
                    return "Empty";

                return sb.ToString();
            }
        }
        public DuelPlayer Find(Mobile mob)
        {
            if (mob is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)mob;

                if (pm.DuelContext == this.m_Context && pm.DuelPlayer.Participant == this)
                    return pm.DuelPlayer;

                return null;
            }

            for (int i = 0; i < this.m_Players.Length; ++i)
            {
                if (this.m_Players[i] != null && this.m_Players[i].Mobile == mob)
                    return this.m_Players[i];
            }

            return null;
        }

        public bool Contains(Mobile mob)
        {
            return (this.Find(mob) != null);
        }

        public void Broadcast(int hue, string message, string nonLocalOverhead, string localOverhead)
        {
            for (int i = 0; i < this.m_Players.Length; ++i)
            {
                if (this.m_Players[i] != null)
                {
                    if (message != null)
                        this.m_Players[i].Mobile.SendMessage(hue, message);

                    if (nonLocalOverhead != null)
                        this.m_Players[i].Mobile.NonlocalOverheadMessage(Network.MessageType.Regular, hue, false, String.Format(nonLocalOverhead, this.m_Players[i].Mobile.Name, this.m_Players[i].Mobile.Female ? "her" : "his"));

                    if (localOverhead != null)
                        this.m_Players[i].Mobile.LocalOverheadMessage(Network.MessageType.Regular, hue, false, localOverhead);
                }
            }
        }

        public void Nullify(DuelPlayer player)
        {
            if (player == null)
                return;

            int index = Array.IndexOf(this.m_Players, player);

            if (index == -1)
                return;

            this.m_Players[index] = null;
        }

        public void Remove(DuelPlayer player)
        {
            if (player == null)
                return;

            int index = Array.IndexOf(this.m_Players, player);

            if (index == -1)
                return;

            DuelPlayer[] old = this.m_Players;
            this.m_Players = new DuelPlayer[old.Length - 1];

            for (int i = 0; i < index; ++i)
                this.m_Players[i] = old[i];

            for (int i = index + 1; i < old.Length; ++i)
                this.m_Players[i - 1] = old[i];
        }

        public void Remove(Mobile player)
        {
            this.Remove(this.Find(player));
        }

        public void Add(Mobile player)
        {
            if (this.Contains(player))
                return;

            for (int i = 0; i < this.m_Players.Length; ++i)
            {
                if (this.m_Players[i] == null)
                {
                    this.m_Players[i] = new DuelPlayer(player, this);
                    return;
                }
            }

            this.Resize(this.m_Players.Length + 1);
            this.m_Players[this.m_Players.Length - 1] = new DuelPlayer(player, this);
        }

        public void Resize(int count)
        {
            DuelPlayer[] old = this.m_Players;
            this.m_Players = new DuelPlayer[count];

            if (old != null)
            {
                int ct = 0;

                for (int i = 0; i < old.Length; ++i)
                {
                    if (old[i] != null && ct < count)
                        this.m_Players[ct++] = old[i];
                }
            }
        }
    }

    public class DuelPlayer
    {
        private readonly Mobile m_Mobile;
        private bool m_Eliminated;
        private bool m_Ready;
        private Participant m_Participant;
        public DuelPlayer(Mobile mob, Participant p)
        {
            this.m_Mobile = mob;
            this.m_Participant = p;

            if (mob is PlayerMobile)
                ((PlayerMobile)mob).DuelPlayer = this;
        }

        public Mobile Mobile
        {
            get
            {
                return this.m_Mobile;
            }
        }
        public bool Ready
        {
            get
            {
                return this.m_Ready;
            }
            set
            {
                this.m_Ready = value;
            }
        }
        public bool Eliminated
        {
            get
            {
                return this.m_Eliminated;
            }
            set
            {
                this.m_Eliminated = value;
                if (this.m_Participant.Context.m_Tournament != null && this.m_Eliminated)
                {
                    this.m_Participant.Context.m_Tournament.OnEliminated(this);
                    this.m_Mobile.SendEverything();
                }
            }
        }
        public Participant Participant
        {
            get
            {
                return this.m_Participant;
            }
            set
            {
                this.m_Participant = value;
            }
        }
    }
}