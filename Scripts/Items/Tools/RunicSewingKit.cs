using Server.Engines.Craft;

namespace Server.Items
{
    public class RunicSewingKit : BaseRunicTool
    {
        [Constructable]
        public RunicSewingKit(CraftResource resource)
            : base(resource, 0xF9D)
        {
            Weight = 2.0;
            Hue = CraftResources.GetHue(resource);
        }

        [Constructable]
        public RunicSewingKit(CraftResource resource, int uses)
            : base(resource, uses, 0xF9D)
        {
            Weight = 2.0;
            Hue = CraftResources.GetHue(resource);
        }

        public RunicSewingKit(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem => DefTailoring.CraftSystem;
        public override void AddNameProperty(ObjectPropertyList list)
        {
            string v = " ";

            if (!CraftResources.IsStandard(Resource))
            {
                int num = CraftResources.GetLocalizationNumber(Resource);

                if (num > 0)
                    v = string.Format("#{0}", num);
                else
                    v = CraftResources.GetName(Resource);
            }

            list.Add(1061119, v); // ~1_LEATHER_TYPE~ runic sewing kit
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

            if (ItemID == 0x13E4 || ItemID == 0x13E3)
                ItemID = 0xF9D;
        }
    }
}
