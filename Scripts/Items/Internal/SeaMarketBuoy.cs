namespace Server.Items
{
    public class SeaMarketBuoy : BaseAddon
    {
        [Constructable]
        public SeaMarketBuoy()
        {
            AddonComponent comp1 = new AddonComponent(18102)
            {
                Name = "buoy"
            };

            AddonComponent comp2 = new AddonComponent(18103);
            comp1.Name = "buoy";

            AddonComponent comp3 = new AddonComponent(18104);
            comp1.Name = "buoy";

            AddonComponent comp4 = new AddonComponent(18105);
            comp1.Name = "buoy";

            AddComponent(comp1, 0, 0, 0);
            AddComponent(comp2, 0, -1, 0);
            AddComponent(comp3, -1, -1, 0);
            AddComponent(comp4, -1, 0, 0);
        }

        public SeaMarketBuoy(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}