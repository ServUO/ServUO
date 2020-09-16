namespace Server.Items
{
    [Flipable(0x4204, 0x4208)]
    public class LargePlateShield : BaseShield
    {
        [Constructable]
        public LargePlateShield()
            : base(0x4204)
        {
            Weight = 8.0;
        }

        public LargePlateShield(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 1;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 0;
        public override int InitMinHits => 50;
        public override int InitMaxHits => 65;
        public override int StrReq => 90;

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
