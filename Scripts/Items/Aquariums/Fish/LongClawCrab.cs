namespace Server.Items
{
    public class LongClawCrab : BaseFish
    {
        [Constructable]
        public LongClawCrab()
            : base(0x3AFC)
        {
            Hue = 0x527;
        }

        public LongClawCrab(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073827;// A Long Claw Crab 
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