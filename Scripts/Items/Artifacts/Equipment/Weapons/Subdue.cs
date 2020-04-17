namespace Server.Items
{
    public class Subdue : Scythe
    {
        public override bool IsArtifact => true;
        [Constructable]
        public Subdue()
        {
            Hue = 0x2cb;
            Attributes.SpellChanneling = 1;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 50;
            Attributes.AttackChance = 10;
            WeaponAttributes.HitLeechMana = 100;
            WeaponAttributes.UseBestSkill = 1;
        }

        public Subdue(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094930;// Subdue [Replica]
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