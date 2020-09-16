namespace Server.Items
{
    [Flipable(0x4202, 0x420A)]
    public class SmallPlateShield : BaseShield
    {
        [Constructable]
        public SmallPlateShield()
            : base(0x4202)
        {
            Weight = 6.0;
        }

        public SmallPlateShield(Serial serial)
            : base(serial)
        {
        }

        public override int BasePhysicalResistance => 0;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 1;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 0;
        public override int InitMinHits => 25;
        public override int InitMaxHits => 30;
        public override int StrReq => 35;

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
