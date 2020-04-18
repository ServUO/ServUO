namespace Server.Items
{
    public class LargeGargoyleBedSouthAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new LargeGargoyleBedSouthDeed();

        #region Mondain's Legacy
        public override bool RetainDeedHue => true;
        #endregion

        [Constructable]
        public LargeGargoyleBedSouthAddon()
        {
            //Left Side
            AddComponent(new AddonComponent(0x4010), 0, 0, 0);
            AddComponent(new AddonComponent(0x4013), 0, 1, 0);
            AddComponent(new AddonComponent(0x4016), 0, 2, 0);
            //Middle
            AddComponent(new AddonComponent(0x4011), 1, 0, 0);
            AddComponent(new AddonComponent(0x4014), 1, 1, 0);
            AddComponent(new AddonComponent(0x4017), 1, 2, 0);
            //Right Side
            AddComponent(new AddonComponent(0x4012), 2, 0, 0);
            AddComponent(new AddonComponent(0x4015), 2, 1, 0);
            AddComponent(new AddonComponent(0x4018), 2, 2, 0);
        }

        public LargeGargoyleBedSouthAddon(Serial serial)
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

    public class LargeGargoyleBedSouthDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new LargeGargoyleBedSouthAddon();
        public override int LabelNumber => 1111761;// large gargish bed (south)

        [Constructable]
        public LargeGargoyleBedSouthDeed()
        {
        }

        public LargeGargoyleBedSouthDeed(Serial serial)
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