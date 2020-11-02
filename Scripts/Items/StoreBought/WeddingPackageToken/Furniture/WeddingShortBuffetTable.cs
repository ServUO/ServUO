using Server.Gumps;

namespace Server.Items
{
    public class WeddingShortBuffetTableAddon : BaseAddon, IDyable
    {
        [Constructable]
        public WeddingShortBuffetTableAddon(bool south)
        {
            if (south)
            {
                AddComponent(new AddonComponent(40654), 0, 0, 0);
                AddComponent(new AddonComponent(40655), -1, 0, 0);
                AddComponent(new AddonComponent(40656), -2, 0, 0);
            }
            else
            {
                AddComponent(new AddonComponent(40657), 0, 0, 0);
                AddComponent(new AddonComponent(40658), 0, -1, 0);
                AddComponent(new AddonComponent(40659), 0, -2, 0);
            }
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;
            return true;
        }

        public WeddingShortBuffetTableAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed { get { return new WeddingShortBuffetTableDeed(); } }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class WeddingShortBuffetTableDeed : BaseAddonDeed, IRewardOption
    {
        public override int LabelNumber => 1157340; // Small Fancy Buffet Table

        [Constructable]
        public WeddingShortBuffetTableDeed()
        {
            LootType = LootType.Blessed;
        }

        public WeddingShortBuffetTableDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new WeddingShortBuffetTableAddon(m_South);

        private bool m_South;

        public void GetOptions(RewardOptionList list)
        {
            list.Add(0, 1116332); // South 
            list.Add(1, 1116333); // East
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            m_South = choice == 0;

            if (!Deleted)
                base.OnDoubleClick(from);
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1154194)); // Choose a Facing:
            }
            else
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.       	
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
