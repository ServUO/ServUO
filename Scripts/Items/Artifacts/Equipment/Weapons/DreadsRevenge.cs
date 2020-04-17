namespace Server.Items
{
    public class DreadsRevenge : Kryss
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1072092; // Dread's Revenge
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public DreadsRevenge()
            : base()
        {
            Hue = 0x3A;
            SkillBonuses.SetValues(0, SkillName.Fencing, 20.0);
            WeaponAttributes.HitPoisonArea = 30;
            Attributes.AttackChance = 15;
            Attributes.WeaponSpeed = 50;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = cold = nrgy = chaos = direct = 0;
            pois = 100;
        }

        public DreadsRevenge(Serial serial)
            : base(serial)
        {
        }

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