namespace Server.Items
{
    public class NavigatorsWorldMap : WorldMap
    {
        public override int LabelNumber => 1075500;  // Navigator's World Map

        [Constructable]
        public NavigatorsWorldMap()
        {
            ItemID = 0x14EB;
            LootType = LootType.Blessed;
            Hue = 483;

            SetDisplay(0, 0, 5119, 4095, 200, 200);
        }

        public NavigatorsWorldMap(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class WorldMap : MapItem
    {
        [Constructable]
        public WorldMap()
        {
            SetDisplay(0, 0, 5119, 4095, 400, 400);
        }

        public override void CraftInit(Mobile from)
        {
            // Unlike the others, world map is not based on crafted location
            Facet = from.Map;

            double skillValue = from.Skills[SkillName.Cartography].Value;
            int x20 = (int)(skillValue * 20);
            int size = 25 + (int)(skillValue * 6.6);

            if (size < 200)
                size = 200;
            else if (size > 400)
                size = 400;

            if (Facet == Map.Trammel || Facet == Map.Felucca)
            {
                if (Spells.SpellHelper.IsAnyT2A(Facet, from.Location))
                {
                    Bounds = new Rectangle2D(5120, 2304, 1024, 1792);
                    Width = size;
                    Height = size;
                }
                else
                    SetDisplay(1344 - x20, 1600 - x20, 1472 + x20, 1728 + x20, size, size);
            }
            else
                SetDisplayByFacet();
        }

        public override int LabelNumber => 1015233;  // world map

        public WorldMap(Serial serial) : base(serial)
        {
        }

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
