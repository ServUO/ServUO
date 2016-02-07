using System;
using Server.Mobiles;
using Server.Network;

namespace Server.Factions
{
    public class FactionStone : BaseSystemController
    {
        private Faction m_Faction;
        [Constructable]
        public FactionStone()
            : this(null)
        {
        }

        [Constructable]
        public FactionStone(Faction faction)
            : base(0xEDC)
        {
            this.Movable = false;
            this.Faction = faction;
        }

        public FactionStone(Serial serial)
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

                this.AssignName(this.m_Faction == null ? null : this.m_Faction.Definition.FactionStoneName);
            }
        }
        public override string DefaultName
        {
            get
            {
                return "faction stone";
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Faction == null)
                return;

            if (!from.InRange(this.GetWorldLocation(), 2))
            {
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
            }
            else if (FactionGump.Exists(from))
            {
                from.SendLocalizedMessage(1042160); // You already have a faction menu open.
            }
            else if (from is PlayerMobile)
            {
                Faction existingFaction = Faction.Find(from);

                if (existingFaction == this.m_Faction || from.AccessLevel >= AccessLevel.GameMaster)
                {
                    PlayerState pl = PlayerState.Find(from);

                    if (pl != null && pl.IsLeaving)
                        from.SendLocalizedMessage(1005051); // You cannot use the faction stone until you have finished quitting your current faction
                    else
                        from.SendGump(new FactionStoneGump((PlayerMobile)from, this.m_Faction));
                }
                else if (existingFaction != null)
                {
                    // TODO: Validate
                    from.SendLocalizedMessage(1005053); // This is not your faction stone!
                }
                else
                {
                    from.SendGump(new JoinStoneGump((PlayerMobile)from, this.m_Faction));
                }
            }
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