namespace Server.Items
{
    public class Pacify : Pike
    {
        public override bool IsArtifact => true;
        [Constructable]
        public Pacify()
        {
            Hue = 0x835;
            Attributes.SpellChanneling = 1;
            Attributes.AttackChance = 10;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.HitLeechMana = 100;
            WeaponAttributes.UseBestSkill = 1;
        }

        public Pacify(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1094929;// Pacify [Replica]
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