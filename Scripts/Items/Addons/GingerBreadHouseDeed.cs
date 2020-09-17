namespace Server.Items
{
    public class GingerBreadHouseAddon : BaseAddon
    {
        public GingerBreadHouseAddon()
        {
            for (int i = 0x2be5; i < 0x2be8; i++)
            {
                LocalizedAddonComponent laoc = new LocalizedAddonComponent(i, 1077395)
                {
                    Light = LightType.SouthSmall
                }; // Gingerbread House
                AddComponent(laoc, (i == 0x2be5) ? -1 : 0, (i == 0x2be7) ? -1 : 0, 0);
            }
        }

        public GingerBreadHouseAddon(Serial serial)
            : base(serial)
        {
        }

        public override BaseAddonDeed Deed => new GingerBreadHouseDeed();
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

    public class GingerBreadHouseDeed : BaseAddonDeed
    {
        [Constructable]
        public GingerBreadHouseDeed()
        {
            Weight = 1.0;
            LootType = LootType.Blessed;
        }

        public GingerBreadHouseDeed(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1077394;//a Gingerbread House Deed
        public override BaseAddon Addon => new GingerBreadHouseAddon();
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