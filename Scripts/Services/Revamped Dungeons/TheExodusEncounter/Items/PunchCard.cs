namespace Server.Items
{
    public class PunchCard : BaseDecayingItem
    {
        [Constructable]
        public PunchCard()
            : base(0x0FF4)
        {
            LootType = LootType.Regular;
            Hue = Utility.RandomNondyedHue();
            Weight = 2;
        }

        public override int Lifespan => 21600;
        public override bool UseSeconds => false;

        public override int LabelNumber => 1153867;  // Punch Card

        public PunchCard(Serial serial)
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