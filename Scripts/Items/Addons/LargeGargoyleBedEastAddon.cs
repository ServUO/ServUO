namespace Server.Items
{
    public class LargeGargoyleBedEastAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new LargeGargoyleBedEastDeed();

        #region Mondain's Legacy
        public override bool RetainDeedHue => true;
        #endregion

        [Constructable]
        public LargeGargoyleBedEastAddon()
        {
            //Left Side
            AddComponent(new AddonComponent(0x4019), 0, 0, 0);
            AddComponent(new AddonComponent(0x401C), 0, 1, 0);
            AddComponent(new AddonComponent(0x401F), 0, 2, 0);
            //Middle
            AddComponent(new AddonComponent(0x401A), 1, 0, 0);
            AddComponent(new AddonComponent(0x401D), 1, 1, 0);
            AddComponent(new AddonComponent(0x4020), 1, 2, 0);
            //Right Side
            AddComponent(new AddonComponent(0x401B), 2, 0, 0);
            AddComponent(new AddonComponent(0x401E), 2, 1, 0);
            AddComponent(new AddonComponent(0x4021), 2, 2, 0);
        }

        public LargeGargoyleBedEastAddon(Serial serial)
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

    public class LargeGargoyleBedEastDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new LargeGargoyleBedEastAddon();
        public override int LabelNumber => 1111762;// large gargish bed (east)

        [Constructable]
        public LargeGargoyleBedEastDeed()
        {
        }

        public LargeGargoyleBedEastDeed(Serial serial)
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
}