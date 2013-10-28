using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Factions
{
    public class JoinStone : BaseSystemController
    {
        private Faction m_Faction;
        [Constructable]
        public JoinStone()
            : this(null)
        {
        }

        [Constructable]
        public JoinStone(Faction faction)
            : base(0xEDC)
        {
            this.Movable = false;
            this.Faction = faction;
        }

        public JoinStone(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Faction Faction
        {
            get
            {
                return this.m_Faction;
            }
            set
            {
                this.m_Faction = value;

                this.Hue = (this.m_Faction == null ? 0 : this.m_Faction.Definition.HueJoin);
                this.AssignName(this.m_Faction == null ? null : this.m_Faction.Definition.SignupName);
            }
        }
        public override string DefaultName
        {
            get
            {
                return "faction signup stone";
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Faction == null)
                return;

            if (!from.InRange(this.GetWorldLocation(), 2))
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            else if (FactionGump.Exists(from))
                from.SendLocalizedMessage(1042160); // You already have a faction menu open.
            else if (Faction.Find(from) == null && from is PlayerMobile)
                from.SendGump(new JoinStoneGump((PlayerMobile)from, this.m_Faction));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            Faction.WriteReference(writer, this.m_Faction);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.Faction = Faction.ReadReference(reader);
                        break;
                    }
            }
        }
    }
}