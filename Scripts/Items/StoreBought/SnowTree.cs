using Server.Gumps;

namespace Server.Items
{
    public class SnowTreeAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new SnowTreeDeed();

        [Constructable]
        public SnowTreeAddon(bool trunk)
        {
            AddComponent(new LocalizedAddonComponent(0xCE0, 1071103), 0, 0, 0);

            if (!trunk)
            {
                var comp = new LocalizedAddonComponent(0xD9D, 1071103)
                {
                    Hue = 1153
                };
                AddComponent(comp, 0, 0, 4);
            }
        }

        public SnowTreeAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            if (version == 0)
            {
                Timer.DelayCall(FixTree);
            }
        }

        private void FixTree()
        {
            AddonComponent toDelete = null;

            foreach (var comp in Components)
            {
                if (comp.ItemID == 0xDA0)
                {
                    comp.ItemID = 0xCE0;
                }

                if (comp.ItemID == 0xD9D)
                {
                    toDelete = comp;
                }
            }

            if (toDelete != null)
            {
                Components.Remove(toDelete);
                toDelete.Addon = null;
                toDelete.Delete();

                var comp = new LocalizedAddonComponent(0xD9D, 1071103)
                {
                    Hue = 1153
                };
                AddComponent(comp, 0, 0, 4);
            }
        }
    }

    public class SnowTreeDeed : BaseAddonDeed, IRewardOption
    {
        public override BaseAddon Addon => new SnowTreeAddon(m_Trunk);
        public override int LabelNumber => 1071103;  // Snow Tree

        private bool m_Trunk;

        [Constructable]
        public SnowTreeDeed()
        {
            LootType = LootType.Blessed;
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add(0, 1071103); // Snow Tree
            list.Add(1, 1071300); // Snow Tree (trunk)
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

        public SnowTreeDeed(Serial serial)
            : base(serial)
        {
        }

        private void SendTarget(Mobile m)
        {
            base.OnDoubleClick(m);
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
