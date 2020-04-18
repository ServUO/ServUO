namespace Server.Items
{
    public class ForTheLifeOfBritanniaSash : BodySash
    {
        public override int LabelNumber => 1075792;  // For the Life of Britannia Sash

        [Constructable]
        public ForTheLifeOfBritanniaSash()
        {
        }

        public ForTheLifeOfBritanniaSash(Serial serial)
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