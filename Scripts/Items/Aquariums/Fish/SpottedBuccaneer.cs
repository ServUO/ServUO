namespace Server.Items
{
    public class SpottedBuccaneer : BaseFish
    {
        [Constructable]
        public SpottedBuccaneer()
            : base(0x3B09)
        {
        }

        public SpottedBuccaneer(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073833;// A Spotted Buccaneer
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