namespace Server.Items
{
    public class Feathernock : BaseQuiver
    {
        public override bool IsArtifact => true;
        [Constructable]
        public Feathernock()
            : base()
        {
            this.SetHue = 0x594;

            this.Attributes.WeaponDamage = 10;
            this.WeightReduction = 30;

            this.SetAttributes.AttackChance = 15;
            this.SetAttributes.BonusDex = 8;
            this.SetAttributes.WeaponSpeed = 30;
            this.SetAttributes.WeaponDamage = 20;
        }

        public Feathernock(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1074324;// Feathernock (Marksman Set)
        public override SetItem SetID => SetItem.Marksman;
        public override int Pieces => 2;
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