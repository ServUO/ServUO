namespace Server.Items
{
    public class BansheesCall : Cyclone
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113529;  // Banshee's Call

        [Constructable]
        public BansheesCall()
        {
            Hue = 1266;
            WeaponAttributes.HitHarm = 40;
            Attributes.BonusStr = 5;
            WeaponAttributes.HitLeechHits = 45;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
            Velocity = 35;
            AosElementDamages.Cold = 100;
        }

        public BansheesCall(Serial serial)
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
