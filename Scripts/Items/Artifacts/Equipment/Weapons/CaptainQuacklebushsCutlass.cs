namespace Server.Items
{
    public class CaptainQuacklebushsCutlass : Cutlass
    {
        public override bool IsArtifact => true;
        [Constructable]
        public CaptainQuacklebushsCutlass()
        {
            Hue = 0x66C;
            Attributes.BonusDex = 5;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.UseBestSkill = 1;
        }

        public CaptainQuacklebushsCutlass(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1063474;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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