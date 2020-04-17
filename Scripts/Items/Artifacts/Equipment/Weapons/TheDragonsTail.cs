namespace Server.Items
{
    public class TheDragonsTail : NoDachi
    {
        public override bool IsArtifact => true;
        [Constructable]
        public TheDragonsTail()
        {
            LootType = LootType.Blessed;
            WeaponAttributes.HitLeechStam = 16;
            Attributes.WeaponSpeed = 10;
            Attributes.WeaponDamage = 25;
        }

        public TheDragonsTail(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1078015;// The Dragon's Tail
        public override int InitMinHits => 80;
        public override int InitMaxHits => 80;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}