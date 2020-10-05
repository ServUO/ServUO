using Server.Network;
using System;

namespace Server.Items
{
    [Flipable(0x2A65, 0x2A67)]
    public class UnsettlingPortraitComponent : AddonComponent
    {
        public UnsettlingPortraitComponent()
            : base(0x2A65)
        {
            TimerRegistry.Register("UnsettlingPortrait", this, TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3), false, p => p.ChangeDirection());
        }

        public UnsettlingPortraitComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074480;// Unsettling portrait
        public override void OnDoubleClick(Mobile from)
        {
            if (Utility.InRange(Location, from.Location, 2))
                Effects.PlaySound(Location, Map, Utility.RandomMinMax(0x567, 0x568));
            else
                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
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

            TimerRegistry.Register("UnsettlingPortrait", this, TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3), false, p => p.ChangeDirection());
        }

        private void ChangeDirection()
        {
            if (ItemID == 0x2A65)
                ItemID += 1;
            else if (ItemID == 0x2A66)
                ItemID -= 1;
            else if (ItemID == 0x2A67)
                ItemID += 1;
            else if (ItemID == 0x2A68)
                ItemID -= 1;
        }
    }

    public class UnsettlingPortraitAddon : BaseAddon
    {
        [Constructable]
        public UnsettlingPortraitAddon()
            : base()
        {
            AddComponent(new UnsettlingPortraitComponent(), 0, 0, 0);
        }

        public UnsettlingPortraitAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new UnsettlingPortraitDeed();
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

    public class UnsettlingPortraitDeed : BaseAddonDeed
    {
        [Constructable]
        public UnsettlingPortraitDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public UnsettlingPortraitDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new UnsettlingPortraitAddon();
        public override int LabelNumber => 1074480;// Unsettling portrait
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
