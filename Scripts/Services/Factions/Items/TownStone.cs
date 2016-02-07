using System;
using Server.Mobiles;

namespace Server.Factions
{
    public class TownStone : BaseSystemController
    {
        private Town m_Town;
        [Constructable]
        public TownStone()
            : this(null)
        {
        }

        [Constructable]
        public TownStone(Town town)
            : base(0xEDE)
        {
            this.Movable = false;
            this.Town = town;
        }

        public TownStone(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.Counselor, AccessLevel.Administrator)]
        public Town Town
        {
            get
            {
                return this.m_Town;
            }
            set
            {
                this.m_Town = value;

                this.AssignName(this.m_Town == null ? null : this.m_Town.Definition.TownStoneName);
            }
        }
        public override string DefaultName
        {
            get
            {
                return "faction town stone";
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_Town == null)
                return;

            Faction faction = Faction.Find(from);

            if (faction == null && from.AccessLevel < AccessLevel.GameMaster)
                return; // TODO: Message?

            if (this.m_Town.Owner == null || (from.AccessLevel < AccessLevel.GameMaster && faction != this.m_Town.Owner))
                from.SendLocalizedMessage(1010332); // Your faction does not control this town
            else if (!this.m_Town.Owner.IsCommander(from))
                from.SendLocalizedMessage(1005242); // Only faction Leaders can use townstones
            else if (FactionGump.Exists(from))
                from.SendLocalizedMessage(1042160); // You already have a faction menu open.
            else if (from is PlayerMobile)
                from.SendGump(new TownStoneGump((PlayerMobile)from, this.m_Town.Owner, this.m_Town));
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            Town.WriteReference(writer, this.m_Town);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.Town = Town.ReadReference(reader);
                        break;
                    }
            }
        }
    }
}