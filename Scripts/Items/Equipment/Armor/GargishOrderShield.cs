namespace Server.Items
{
    [Flipable(0x422A, 0x422C)]
    public class GargishOrderShield : BaseShield
    {
        [Constructable]
        public GargishOrderShield()
            : base(0x422A)
        {
            Weight = 7.0;
        }

        public GargishOrderShield(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 1;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 0;
        public override int InitMinHits => 100;
        public override int InitMaxHits => 125;
        public override int StrReq => 95;

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); //version
        }
    }
}
