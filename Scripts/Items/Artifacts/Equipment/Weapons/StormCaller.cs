namespace Server.Items
{
    public class StormCaller : Boomerang
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113530;  // Storm Caller

        [Constructable]
        public StormCaller()
            : base()
        {
            Hue = 456;
            WeaponAttributes.BattleLust = 1;
            Attributes.BonusStr = 5;
            WeaponAttributes.HitLightning = 40;
            WeaponAttributes.HitLowerDefend = 30;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 40;
            AosElementDamages.Physical = 20;
            AosElementDamages.Fire = 20;
            AosElementDamages.Cold = 20;
            AosElementDamages.Poison = 20;
            AosElementDamages.Energy = 20;
        }

        public StormCaller(Serial serial)
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