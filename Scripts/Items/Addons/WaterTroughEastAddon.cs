namespace Server.Items
{
    public class WaterTroughEastAddon : BaseAddon, IWaterSource
    {
        [Constructable]
        public WaterTroughEastAddon()
        {
            AddComponent(new AddonComponent(0xB41), 0, 0, 0);
            AddComponent(new AddonComponent(0xB42), 0, 1, 0);
        }

        public WaterTroughEastAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new WaterTroughEastDeed();
        public int Quantity
        {
            get
            {
                return 500;
            }
            set
            {
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class WaterTroughEastDeed : BaseAddonDeed
    {
        [Constructable]
        public WaterTroughEastDeed()
        {
        }

        public WaterTroughEastDeed(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddon Addon => new WaterTroughEastAddon();
        public override int LabelNumber => 1044349;// water trough (east)
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}