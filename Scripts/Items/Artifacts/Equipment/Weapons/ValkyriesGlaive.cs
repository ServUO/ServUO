namespace Server.Items
{
    public class ValkyriesGlaive : SoulGlaive
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113531;  // Valkyrie's Glaive

        [Constructable]
        public ValkyriesGlaive()
        {
            Attributes.SpellChanneling = 1;
            Slayer = SlayerName.Silver;
            WeaponAttributes.HitFireball = 40;
            Attributes.BonusStr = 5;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 20;
            Hue = 1651; //Hue not exact
        }

        public ValkyriesGlaive(Serial serial)
            : base(serial)
        {
        }
        public override int ArtifactRarity => 5;
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