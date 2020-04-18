namespace Server.Items
{
    public class Craven : DualPointedSpear
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1154474;  // Craven

        [Constructable]
        public Craven()
        {
            Slayer2 = BaseRunicTool.GetRandomSlayer();
            WeaponAttributes.HitLowerAttack = 40;
            Attributes.WeaponSpeed = 26;
            Attributes.WeaponDamage = 35;
            Attributes.LowerManaCost = 8;
            Attributes.BalancedWeapon = 1;
            Hue = 1365;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = 70; cold = 30;
            nrgy = pois = chaos = direct = fire = 0;
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public Craven(Serial serial)
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