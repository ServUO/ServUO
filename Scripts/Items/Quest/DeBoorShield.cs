namespace Server.Items
{
    public class DeBoorShield : MetalKiteShield
    {
        public override int LabelNumber => 1075308;  // Ancestral Shield
        public override bool HiddenQuestItemHue => true;

        [Constructable]
        public DeBoorShield()
        {
            LootType = LootType.Blessed;
        }

        public DeBoorShield(Serial serial)
            : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }
    }
}