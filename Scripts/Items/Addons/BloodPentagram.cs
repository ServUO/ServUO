namespace Server.Items
{
    public class BloodPentagram : BaseAddon
    {
        [Constructable]
        public BloodPentagram()
        {
            AddComponent(new AddonComponent(0x1CF9), 0, 1, 0);
            AddComponent(new AddonComponent(0x1CF8), 0, 2, 0);
            AddComponent(new AddonComponent(0x1CF7), 0, 3, 0);
            AddComponent(new AddonComponent(0x1CF6), 0, 4, 0);
            AddComponent(new AddonComponent(0x1CF5), 0, 5, 0);

            AddComponent(new AddonComponent(0x1CFB), 1, 0, 0);
            AddComponent(new AddonComponent(0x1CFA), 1, 1, 0);
            AddComponent(new AddonComponent(0x1D09), 1, 2, 0);
            AddComponent(new AddonComponent(0x1D08), 1, 3, 0);
            AddComponent(new AddonComponent(0x1D07), 1, 4, 0);
            AddComponent(new AddonComponent(0x1CF4), 1, 5, 0);

            AddComponent(new AddonComponent(0x1CFC), 2, 0, 0);
            AddComponent(new AddonComponent(0x1D0A), 2, 1, 0);
            AddComponent(new AddonComponent(0x1D11), 2, 2, 0);
            AddComponent(new AddonComponent(0x1D10), 2, 3, 0);
            AddComponent(new AddonComponent(0x1D06), 2, 4, 0);
            AddComponent(new AddonComponent(0x1CF3), 2, 5, 0);

            AddComponent(new AddonComponent(0x1CFD), 3, 0, 0);
            AddComponent(new AddonComponent(0x1D0B), 3, 1, 0);
            AddComponent(new AddonComponent(0x1D12), 3, 2, 0);
            AddComponent(new AddonComponent(0x1D0F), 3, 3, 0);
            AddComponent(new AddonComponent(0x1D05), 3, 4, 0);
            AddComponent(new AddonComponent(0x1CF2), 3, 5, 0);

            AddComponent(new AddonComponent(0x1CFE), 4, 0, 0);
            AddComponent(new AddonComponent(0x1D0C), 4, 1, 0);
            AddComponent(new AddonComponent(0x1D0D), 4, 2, 0);
            AddComponent(new AddonComponent(0x1D0E), 4, 3, 0);
            AddComponent(new AddonComponent(0x1D04), 4, 4, 0);
            AddComponent(new AddonComponent(0x1CF1), 4, 5, 0);

            AddComponent(new AddonComponent(0x1CFF), 5, 0, 0);
            AddComponent(new AddonComponent(0x1D00), 5, 1, 0);
            AddComponent(new AddonComponent(0x1D01), 5, 2, 0);
            AddComponent(new AddonComponent(0x1D02), 5, 3, 0);
            AddComponent(new AddonComponent(0x1D03), 5, 4, 0);
        }

        public BloodPentagram(Serial serial)
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