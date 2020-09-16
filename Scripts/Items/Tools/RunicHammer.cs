using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x13E4, 0x13E3)]
    public class RunicHammer : BaseRunicTool
    {
        [Constructable]
        public RunicHammer(CraftResource resource)
            : base(resource, 0x13E3)
        {
            Weight = 8.0;
            Layer = Layer.OneHanded;
            Hue = CraftResources.GetHue(resource);
        }

        [Constructable]
        public RunicHammer(CraftResource resource, int uses)
            : base(resource, uses, 0x13E3)
        {
            Weight = 8.0;
            Layer = Layer.OneHanded;
            Hue = CraftResources.GetHue(resource);
        }

        public RunicHammer(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem => DefBlacksmithy.CraftSystem;
        public override int LabelNumber
        {
            get
            {
                int index = CraftResources.GetIndex(Resource);

                if (index >= 1 && index <= 8)
                    return 1049019 + index;

                return 1045128; // runic smithy hammer
            }
        }
        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);

            int index = CraftResources.GetIndex(Resource);

            if (index >= 1 && index <= 8)
                return;

            if (!CraftResources.IsStandard(Resource))
            {
                int num = CraftResources.GetLocalizationNumber(Resource);

                if (num > 0)
                    list.Add(num);
                else
                    list.Add(CraftResources.GetName(Resource));
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
}