using Server.Engines.Craft;

namespace Server.Items
{
    public class RunicMalletAndChisel : BaseRunicTool
    {
        public override CraftSystem CraftSystem => DefMasonry.CraftSystem;

        public override int LabelNumber
        {
            get
            {
                int index = CraftResources.GetIndex(Resource);

                if (index >= 1 && index <= 8)
                    return 1111795 + index;

                return 1045128; // mallet and chisel
            }
        }

        [Constructable]
        public RunicMalletAndChisel(CraftResource resource) : base(resource, 4787)
        {
            Weight = 2.0;
            Hue = CraftResources.GetHue(resource);
        }

        [Constructable]
        public RunicMalletAndChisel(CraftResource resource, int uses) : base(resource, uses, 4787)
        {
            Weight = 2.0;
            Hue = CraftResources.GetHue(resource);
        }

        public RunicMalletAndChisel(Serial serial)
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

            int version = reader.ReadInt();
        }
    }
}