namespace Server.Items
{
    [Flipable(0x4228, 0x4229)]
    public class GargishChaosShield : BaseShield
    {
        public override int BasePhysicalResistance => 1;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 0;
        public override int InitMinHits => 100;
        public override int InitMaxHits => 125;
        public override int StrReq => 95;

        [Constructable]
        public GargishChaosShield()
            : base(0x4228)
        {
            Weight = 5.0;
        }

        public GargishChaosShield(Serial serial)
            : base(serial)
        {
        }

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
