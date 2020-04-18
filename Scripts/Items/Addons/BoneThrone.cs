namespace Server.Items
{
    [Flipable(0x2A58, 0x2A59)]
    public class BoneThroneComponent : AddonComponent
    {
        public BoneThroneComponent()
            : base(0x2A58)
        {
        }

        public BoneThroneComponent(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074476;// Bone throne
        public override bool OnMoveOver(Mobile m)
        {
            bool allow = base.OnMoveOver(m);

            if (allow && m.Alive && m.Player && (m.IsPlayer() || !m.Hidden))
                Effects.PlaySound(Location, Map, Utility.RandomMinMax(0x54B, 0x54D));

            return allow;
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

    public class BoneThroneAddon : BaseAddon
    {
        [Constructable]
        public BoneThroneAddon()
            : base()
        {
            AddComponent(new BoneThroneComponent(), 0, 0, 0);
        }

        public BoneThroneAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new BoneThroneDeed();
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

    public class BoneThroneDeed : BaseAddonDeed
    {
        [Constructable]
        public BoneThroneDeed()
            : base()
        {
            LootType = LootType.Blessed;
        }

        public BoneThroneDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new BoneThroneAddon();
        public override int LabelNumber => 1074476;// Bone throne
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