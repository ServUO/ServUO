namespace Server.Items
{
    public class GargoyleShortTableAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new GargoyleShortTableDeed();

        #region Mondain's Legacy
        public override bool RetainDeedHue => true;
        #endregion

        [Constructable]
        public GargoyleShortTableAddon()
        {
            AddComponent(new AddonComponent(0x4033), 0, 0, 0);
            AddComponent(new AddonComponent(0x4035), 0, 1, 0);
            AddComponent(new AddonComponent(0x4034), 1, 0, 0);
            AddComponent(new AddonComponent(0x4036), 1, 1, 0);
        }

        public GargoyleShortTableAddon(Serial serial)
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

    public class GargoyleShortTableDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new GargoyleShortTableAddon();
        public override int LabelNumber => 1095307;// large gargish bed (south)

        [Constructable]
        public GargoyleShortTableDeed()
        {
        }

        public GargoyleShortTableDeed(Serial serial)
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