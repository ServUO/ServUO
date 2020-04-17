namespace Server.Items
{
    public class ArcticDeathDealer : WarMace
    {
        public override bool IsArtifact => true;
        [Constructable]
        public ArcticDeathDealer()
        {
            Hue = 0x480;
            WeaponAttributes.HitHarm = 33;
            WeaponAttributes.HitLowerAttack = 40;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 40;
            WeaponAttributes.ResistColdBonus = 10;
        }

        public ArcticDeathDealer(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1063481;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            cold = 50;
            phys = 50;

            pois = fire = nrgy = chaos = direct = 0;
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