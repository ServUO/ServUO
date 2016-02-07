using System;
using Server.Mobiles;

namespace Server.Ethics
{
    public class PlayerCollection : System.Collections.ObjectModel.Collection<Player>
    {
    }

    [PropertyObject]
    public class Player
    {
        private readonly Ethic m_Ethic;
        private readonly Mobile m_Mobile;
        private int m_Power;
        private int m_History;
        private Mobile m_Steed;
        private Mobile m_Familiar;
        private DateTime m_Shield;
        public Player(Ethic ethic, Mobile mobile)
        {
            this.m_Ethic = ethic;
            this.m_Mobile = mobile;

            this.m_Power = 5;
            this.m_History = 5;
        }

        public Player(Ethic ethic, GenericReader reader)
        {
            this.m_Ethic = ethic;

            int version = reader.ReadEncodedInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Mobile = reader.ReadMobile();

                        this.m_Power = reader.ReadEncodedInt();
                        this.m_History = reader.ReadEncodedInt();

                        this.m_Steed = reader.ReadMobile();
                        this.m_Familiar = reader.ReadMobile();

                        this.m_Shield = reader.ReadDeltaTime();

                        break;
                    }
            }
        }

        public Ethic Ethic
        {
            get
            {
                return this.m_Ethic;
            }
        }
        public Mobile Mobile
        {
            get
            {
                return this.m_Mobile;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public int Power
        {
            get
            {
                return this.m_Power;
            }
            set
            {
                this.m_Power = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public int History
        {
            get
            {
                return this.m_History;
            }
            set
            {
                this.m_History = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public Mobile Steed
        {
            get
            {
                return this.m_Steed;
            }
            set
            {
                this.m_Steed = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster, AccessLevel.Administrator)]
        public Mobile Familiar
        {
            get
            {
                return this.m_Familiar;
            }
            set
            {
                this.m_Familiar = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsShielded
        {
            get
            {
                if (this.m_Shield == DateTime.MinValue)
                    return false;

                if (DateTime.UtcNow < (this.m_Shield + TimeSpan.FromHours(1.0)))
                    return true;

                this.FinishShield();
                return false;
            }
        }
        public static Player Find(Mobile mob)
        {
            return Find(mob, false);
        }

        public static Player Find(Mobile mob, bool inherit)
        {
            PlayerMobile pm = mob as PlayerMobile;

            if (pm == null)
            {
                if (inherit && mob is BaseCreature)
                {
                    BaseCreature bc = mob as BaseCreature;

                    if (bc != null && bc.Controlled)
                        pm = bc.ControlMaster as PlayerMobile;
                    else if (bc != null && bc.Summoned)
                        pm = bc.SummonMaster as PlayerMobile;
                }

                if (pm == null)
                    return null;
            }

            Player pl = pm.EthicPlayer;

            if (pl != null && !pl.Ethic.IsEligible(pl.Mobile))
                pm.EthicPlayer = pl = null;

            return pl;
        }

        public void BeginShield()
        {
            this.m_Shield = DateTime.UtcNow;
        }

        public void FinishShield()
        {
            this.m_Shield = DateTime.MinValue;
        }

        public void CheckAttach()
        {
            if (this.m_Ethic.IsEligible(this.m_Mobile))
                this.Attach();
        }

        public void Attach()
        {
            if (this.m_Mobile is PlayerMobile)
                (this.m_Mobile as PlayerMobile).EthicPlayer = this;

            this.m_Ethic.Players.Add(this);
        }

        public void Detach()
        {
            if (this.m_Mobile is PlayerMobile)
                (this.m_Mobile as PlayerMobile).EthicPlayer = null;

            this.m_Ethic.Players.Remove(this);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write(this.m_Mobile);

            writer.WriteEncodedInt(this.m_Power);
            writer.WriteEncodedInt(this.m_History);

            writer.Write(this.m_Steed);
            writer.Write(this.m_Familiar);

            writer.WriteDeltaTime(this.m_Shield);
        }
    }
}