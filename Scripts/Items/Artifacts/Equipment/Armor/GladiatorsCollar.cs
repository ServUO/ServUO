namespace Server.Items
{
    public class GladiatorsCollar : PlateGorget
    {
        public override bool IsArtifact => true;
        [Constructable]
        public GladiatorsCollar()
        {
            Hue = 0x26d;
            Attributes.BonusHits = 10;
            Attributes.AttackChance = 10;
            ArmorAttributes.MageArmor = 1;
        }

        public GladiatorsCollar(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094917;// Gladiator's Collar [Replica]
        public override int BasePhysicalResistance => 18;
        public override int BaseFireResistance => 18;
        public override int BaseColdResistance => 17;
        public override int BasePoisonResistance => 18;
        public override int BaseEnergyResistance => 16;
        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;
        public override bool CanFortify => false;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}