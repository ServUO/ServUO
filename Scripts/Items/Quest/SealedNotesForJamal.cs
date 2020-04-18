namespace Server.Items
{
    public class SealedNotesForJamal : Item
    {
        [Constructable]
        public SealedNotesForJamal()
            : base(0xEF9)
        {
            LootType = LootType.Blessed;
            Weight = 5;
        }

        public SealedNotesForJamal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074998;// sealed notes for Jamal
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