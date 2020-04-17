namespace Server.Items
{
    public class StringOfPartsOfParoxysmusVictims : Item
    {
        [Constructable]
        public StringOfPartsOfParoxysmusVictims()
            : base(0xFD2)
        {
        }

        public StringOfPartsOfParoxysmusVictims(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072082;// String of Parts of Paroxysmus' Victims
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