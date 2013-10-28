using System;
using Server.Gumps;

namespace Server.Items
{
    public class PeerlessTeleporter : Teleporter
    { 
        private PeerlessAltar m_Altar;
        [Constructable]
        public PeerlessTeleporter()
            : this(null)
        {
        }

        public PeerlessTeleporter(PeerlessAltar altar)
            : base()
        {
            this.m_Altar = altar;
        }

        public PeerlessTeleporter(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PeerlessAltar Altar
        {
            get
            {
                return this.m_Altar;
            }
            set
            {
                this.m_Altar = value;
            }
        }
        public override bool OnMoveOver(Mobile m)
        { 
            if (m.Alive)
            {
                m.CloseGump(typeof(ConfirmExitGump));
                m.SendGump(new ConfirmExitGump(this.m_Altar));
            }
            else if (this.m_Altar != null)
            {
                this.m_Altar.Exit(m);
                return false;
            }
			
            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
			
            writer.Write((Item)this.m_Altar);
			
            if (this.m_Altar != null && this.m_Altar.Map != this.Map)
                this.Map = this.m_Altar.Map;
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
			
            this.m_Altar = reader.ReadItem() as PeerlessAltar;
        }
    }
}