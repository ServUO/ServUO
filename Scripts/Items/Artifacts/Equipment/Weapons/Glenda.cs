namespace Server.Items
{
    public class Glenda : Club
    {
        public override int LabelNumber => 1157346;  // glenda
        public override bool IsArtifact => true;

        [Constructable]
        public Glenda()
        {
            ExtendedWeaponAttributes.BoneBreaker = 1;
            WeaponAttributes.HitLeechMana = 20;
            WeaponAttributes.HitLowerDefend = 70;
            Attributes.BonusStr = 16;
            Attributes.WeaponDamage = 100;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 100;
            fire = cold = nrgy = chaos = direct = pois = 0;
        }

        public Glenda(Serial serial)
            : base(serial)
        {
        }

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