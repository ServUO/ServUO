namespace Server.Items
{
    public class RuneEngravedPegLeg : Club
    {
        public override int LabelNumber => 1116622;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public RuneEngravedPegLeg()
        {
            WeaponAttributes.HitLightning = 40;
            WeaponAttributes.HitLowerDefend = 40;
            Attributes.RegenHits = 3;
            Attributes.AttackChance = 5;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = cold = nrgy = pois = direct = 0;
            chaos = 100;
        }

        public RuneEngravedPegLeg(Serial serial)
            : base(serial)
        {
        }

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