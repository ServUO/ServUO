namespace Server.Items
{
    public class EmberStaff : QuarterStaff
    {
        public override bool IsArtifact => true;
        [Constructable]
        public EmberStaff()
        {
            LootType = LootType.Blessed;
            WeaponAttributes.HitFireball = 15;
            WeaponAttributes.MageWeapon = 20;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = -1;
            WeaponAttributes.LowerStatReq = 50;
        }

        public EmberStaff(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1077582;// Ember Staff
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