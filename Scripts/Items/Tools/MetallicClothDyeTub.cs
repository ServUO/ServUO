namespace Server.Items
{
    public class MetallicClothDyeTub : DyeTub
    {
        [Constructable]
        public MetallicClothDyeTub()
        {
            LootType = LootType.Blessed;
        }

        public MetallicClothDyeTub(Serial serial)
            : base(serial)
        {
        }

        public override int TargetMessage => 500859;  // Select the clothing to dye.
        public override int FailMessage => 1153977;  // You can only dye cloth with this tub.
        public override int LabelNumber => 1152920;  // Metallic Cloth Dye Tub
        public override CustomHuePicker CustomHuePicker => CustomHuePicker.MetallicDyeTub;

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