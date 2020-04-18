namespace Server.Items
{
    public class AbyssalBlade : StoneWarSword
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113520;  // Abyssal Blade

        [Constructable]
        public AbyssalBlade()
        {
            Hue = 2404;
            WeaponAttributes.HitManaDrain = 50;
            WeaponAttributes.HitFatigue = 50;
            WeaponAttributes.HitLeechHits = 60;
            WeaponAttributes.HitLeechStam = 60;
            WeaponAttributes.HitLeechMana = 60;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 60;
            AosElementDamages.Chaos = 100;
        }

        public AbyssalBlade(Serial serial)
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
