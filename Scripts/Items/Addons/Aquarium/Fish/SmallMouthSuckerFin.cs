namespace Server.Items
{
    public class SmallMouthSuckerFin : BaseFish
    {
        [Constructable]
        public SmallMouthSuckerFin()
            : base(0x3B01)
        {
        }

        public SmallMouthSuckerFin(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074590;// Small Mouth Sucker Fin
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