using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Doom
{
    public class SummoningAltar : AbbatoirAddon
    {
        private BoneDemon m_Daemon;
        [Constructable]
        public SummoningAltar()
        {
        }

        public SummoningAltar(Serial serial)
            : base(serial)
        {
        }

        public BoneDemon Daemon
        {
            get
            {
                return this.m_Daemon;
            }
            set
            {
                this.m_Daemon = value;
                this.CheckDaemon();
            }
        }
        public void CheckDaemon()
        {
            if (this.m_Daemon == null || !this.m_Daemon.Alive)
            {
                this.m_Daemon = null;
                this.Hue = 0;
            }
            else
            {
                this.Hue = 0x66D;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((Mobile)this.m_Daemon);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            this.m_Daemon = reader.ReadMobile() as BoneDemon;

            this.CheckDaemon();
        }
    }
}