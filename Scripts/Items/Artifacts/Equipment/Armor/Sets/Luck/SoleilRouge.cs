namespace Server.Items
{
    public class SoleilRouge : GoldBracelet
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1154371;  // Soleil Rouge
        public override SetItem SetID => SetItem.Luck2;
        public override int Pieces => 2;
        [Constructable]
        public SoleilRouge() : base()
        {
            Weight = 1.0;
            Hue = 1166;

            Attributes.Luck = 150;
            Attributes.AttackChance = 10;
            Attributes.WeaponDamage = 20;

            SetHue = 1166;
            SetAttributes.Luck = 100;
            SetAttributes.AttackChance = 10;
            SetAttributes.WeaponDamage = 20;
            SetAttributes.WeaponSpeed = 10;
            SetAttributes.RegenHits = 2;
            SetAttributes.RegenStam = 3;
        }

        public SoleilRouge(Serial serial) : base(serial)
        {
        }

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
