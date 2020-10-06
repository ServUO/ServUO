using Server.Network;
using System;

namespace Server.Items
{
    [Flipable(0x2A5D, 0x2A61)]
    public class DisturbingPortraitComponent : AddonComponent
    {
        public DisturbingPortraitComponent()
            : base(0x2A5D)
        {
            TimerRegistry.Register("DisturbingPortrait", this, TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3), false, p => p.Change());
        }

        public DisturbingPortraitComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074479;// Disturbing portrait
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

            TimerRegistry.Register("DisturbingPortrait", this, TimeSpan.FromMinutes(3), TimeSpan.FromMinutes(3), false, p => p.Change());
        }

        private void Change()
        {
            if (ItemID < 0x2A61)
                ItemID = Utility.RandomMinMax(0x2A5D, 0x2A60);
            else
                ItemID = Utility.RandomMinMax(0x2A61, 0x2A64);
        }
    }

    public class DisturbingPortraitAddon : BaseAddon
    {
        [Constructable]
        public DisturbingPortraitAddon()
            : base()
        {
            AddComponent(new DisturbingPortraitComponent(), 0, 0, 0);
        }

        public DisturbingPortraitAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new DisturbingPortraitDeed();
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

    public class DisturbingPortraitDeed : BaseAddonDeed
    {
        [Constructable]
        public DisturbingPortraitDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public DisturbingPortraitDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new DisturbingPortraitAddon();
        public override int LabelNumber => 1074479;// Disturbing portrait
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
