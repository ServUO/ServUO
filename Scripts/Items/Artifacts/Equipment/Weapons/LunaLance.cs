namespace Server.Items
{
    public class LunaLance : Lance
    {
        public override bool IsArtifact => true;
        [Constructable]
        public LunaLance()
        {
            Hue = 0x47E;
            SkillBonuses.SetValues(0, SkillName.Chivalry, 10.0);
            Attributes.BonusStr = 5;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 35;
            WeaponAttributes.UseBestSkill = 1;
        }

        public LunaLance(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1063469;
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