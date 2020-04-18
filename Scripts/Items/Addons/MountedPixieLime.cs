using Server.Network;

namespace Server.Items
{
    [Flipable(0x2A77, 0x2A78)]
    public class MountedPixieLimeComponent : AddonComponent
    {
        public MountedPixieLimeComponent()
            : base(0x2A77)
        {
        }

        public MountedPixieLimeComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074482;// Mounted pixie
        public override void OnDoubleClick(Mobile from)
        {
            if (Utility.InRange(Location, from.Location, 2))
                Effects.PlaySound(Location, Map, Utility.RandomMinMax(0x55F, 0x561));
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

    public class MountedPixieLimeAddon : BaseAddon
    {
        public MountedPixieLimeAddon()
            : base()
        {
            AddComponent(new MountedPixieLimeComponent(), 0, 0, 0);
        }

        public MountedPixieLimeAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new MountedPixieLimeDeed();
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

    public class MountedPixieLimeDeed : BaseAddonDeed
    {
        [Constructable]
        public MountedPixieLimeDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public MountedPixieLimeDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new MountedPixieLimeAddon();
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