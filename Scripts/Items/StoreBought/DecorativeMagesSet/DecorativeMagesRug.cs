using Server.Gumps;

namespace Server.Items
{
    public class DecorativeMagesRugAddon : BaseAddon, IDyable
    {
        private static readonly int[,] _Large =
        {
            {42400, 2, 0, 0}, {42395, 2, 1, 0}, {42410, 2, -2, 0},
            {42405, 2, -1, 0}, {42399, 1, 0, 0}, {42398, 0, 0, 0},	
            {42393, 0, 1, 0}, {42390, 1, 2, 0}, {42394, 1, 1, 0},	
            {42402, -1, 0, 0}, {42397, -1, 1, 0}, {42411, -2, 2, 0},	
            {42401, -2, 0, 0}, {42396, -2, 1, 0}, {42392, -1, 2, 0},	
            {42389, 0, 2, 0}, {42409, -1, -2, 0}, {42408, 0, -2, 0},	
            {42391, 1, -2, 0}, {42404, 1, -1, 0}, {42403, 0, -1, 0},
            {42407, -1, -1, 0}, {42406, -2, -1, 0}
		};

        private static readonly int[,] _Small =
        {
            {42417, 1, 0, 0}, {42414, 1, 1, 0}, {42456, 1, -1, 0},
            {42454, 0, 0, 0}, {42458, -1, -1, 0}, {42457, 0, -1, 0},
            {42455, -1, 0, 0}, {42416, -1, 1, 0}, {42415, 0, 1, 0}
		};

        public override BaseAddonDeed Deed => new DecorativeMagesRugAddonDeed();

        [Constructable]
        public DecorativeMagesRugAddon(ItemSize size)
        {
            int[,] list;

            switch (size)
            {
                default: list = _Small; break;
                case ItemSize.Large: list = _Large; break;
            }

            for (int i = 0; i < list.Length / 4; i++)
                AddComponent(new AddonComponent(list[i, 0]), list[i, 1], list[i, 2], list[i, 3]);
        }

        public DecorativeMagesRugAddon(Serial serial)
            : base(serial)
        {
        }

        public override bool RetainDeedHue => true;

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;
            return true;
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class DecorativeMagesRugAddonDeed : BaseAddonDeed, IRewardOption, IDyable
    {
        public override int LabelNumber => 1159522;  // Decorative Mage's Rug

        private ItemSize _Size;

        public override BaseAddon Addon => new DecorativeMagesRugAddon(_Size);

        public override bool ExcludeDeedHue => true;

        [Constructable]
        public DecorativeMagesRugAddonDeed()
        {
            LootType = LootType.Blessed;
        }

        public DecorativeMagesRugAddonDeed(Serial serial)
            : base(serial)
        {
        }

        public virtual bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;
            return true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (IsChildOf(from.Backpack))
            {
                from.CloseGump(typeof(AddonOptionGump));
                from.SendGump(new AddonOptionGump(this, 1159523)); // Choose a size:
            }
            else
                from.SendLocalizedMessage(1042009); // This item must be in your backpack to be used.       	
        }

        public void GetOptions(RewardOptionList list)
        {
            list.Add((int)ItemSize.Small, 1062224); // Small
            list.Add((int)ItemSize.Large, 1062225); // Large
        }

        public void OnOptionSelected(Mobile from, int choice)
        {
            _Size = (ItemSize)choice - 1;

            if (!Deleted && IsChildOf(from.Backpack))
                base.OnDoubleClick(from);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // Version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
             reader.ReadInt();
        }
    }
}
