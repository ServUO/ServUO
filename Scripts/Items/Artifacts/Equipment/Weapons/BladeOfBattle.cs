namespace Server.Items
{
    public class BladeOfBattle : Shortblade
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113525;  // Blade of Battle

        [Constructable]
        public BladeOfBattle()
        {
            Hue = 2045;
            WeaponAttributes.HitLowerDefend = 40;
            WeaponAttributes.BattleLust = 1;
            Attributes.AttackChance = 15;
            Attributes.DefendChance = 10;
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 50;
        }

        public BladeOfBattle(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

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
