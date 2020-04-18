using Server.Network;

namespace Server.Items
{
    [Flipable(0x2A71, 0x2A72)]
    public class MountedPixieGreenComponent : AddonComponent
    {
        public MountedPixieGreenComponent()
            : base(0x2A71)
        {
        }

        public MountedPixieGreenComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074482;// Mounted pixie
        public override void OnDoubleClick(Mobile from)
        {
            if (Utility.InRange(Location, from.Location, 2))
                Effects.PlaySound(Location, Map, Utility.RandomMinMax(0x554, 0x557));
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
        }
    }

    public class MountedPixieGreenAddon : BaseAddon
    {
        public MountedPixieGreenAddon()
            : base()
        {
            AddComponent(new MountedPixieGreenComponent(), 0, 0, 0);
        }

        public MountedPixieGreenAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new MountedPixieGreenDeed();
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

    public class MountedPixieGreenDeed : BaseAddonDeed
    {
        [Constructable]
        public MountedPixieGreenDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public MountedPixieGreenDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new MountedPixieGreenAddon();
        public override int LabelNumber => 1074482;// Mounted pixie
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