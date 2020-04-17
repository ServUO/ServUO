namespace Server.Items
{
    public class Mangler : Broadsword
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1114842;  // Mangler

        [Constructable]
        public Mangler()
            : base()
        {
            Hue = 2001;
            WeaponAttributes.HitLeechMana = 50;
            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 25;
            WeaponAttributes.HitHarm = 50;
            WeaponAttributes.UseBestSkill = 1;
            WeaponAttributes.HitLowerDefend = 30;
        }

        public Mangler(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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