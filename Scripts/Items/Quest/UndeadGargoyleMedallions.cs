namespace Server.Items
{
    public class UndeadGargoyleMedallions : Item
    {
        public override int LabelNumber => 1112907; // Undead Gargoyle Medallion

        [Constructable]
        public UndeadGargoyleMedallions()
            : base(0x1088)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
            Hue = 0x47F;
        }

        public UndeadGargoyleMedallions(Serial serial)
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
}
