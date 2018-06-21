using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public class MapleTreeAddon : BaseAddon
    {
        public override BaseAddonDeed Deed { get { return new MapleTreeDeed(); } }

        [Constructable]
        public MapleTreeAddon(bool trunk)
        {
            AddComponent(new LocalizedAddonComponent(0x247D, 1071104), 0, 0, 0);

            if (!trunk)
                AddComponent(new LocalizedAddonComponent(0x36A1, 1071104), 0, 0, 0);
        }

        public MapleTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }

    public class MapleTreeDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon { get { return new MapleTreeAddon(m_Trunk); } }
        public override int LabelNumber { get { return 1071104; } } // Maple Tree

        private bool m_Trunk;

        [Constructable]
        public MapleTreeDeed()
        {
            LootType = LootType.Blessed;
        }

        public MapleTreeDeed(Serial serial)
            : base(serial)
        {
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(0, 1071104); // Maple Tree 
            list.Add(1, 1071301); // Maple Tree (trunk)
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            m_Trunk = choice == 1;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(RewardOptionGump));
                from.SendGump(new RewardOptionGump(this));
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}