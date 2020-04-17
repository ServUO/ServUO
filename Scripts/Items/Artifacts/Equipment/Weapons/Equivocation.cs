namespace Server.Items
{
    public class Equivocation : GnarledStaff
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1154473;  // Equivocation

        [Constructable]
        public Equivocation()
        {
            Attributes.BalancedWeapon = 1;
            Slayer2 = BaseRunicTool.GetRandomSlayer();
            Attributes.AttackChance = 10;
            Attributes.RegenHits = 6;
            Attributes.Brittle = 1;
            Attributes.WeaponSpeed = 35;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.HitCurse = 15;
            Hue = 1365;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = pois = 50;
            cold = nrgy = chaos = direct = fire = 0;
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public Equivocation(Serial serial)
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

    public class GargishEquivocation : GargishGnarledStaff
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1154473;  // Equivocation

        [Constructable]
        public GargishEquivocation()
        {
            Attributes.BalancedWeapon = 1;
            Slayer2 = BaseRunicTool.GetRandomSlayer();
            Attributes.AttackChance = 10;
            Attributes.RegenHits = 6;
            Attributes.Brittle = 1;
            Attributes.WeaponSpeed = 35;
            Attributes.WeaponDamage = 50;
            WeaponAttributes.HitCurse = 15;
            Hue = 1365;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = pois = 50;
            cold = nrgy = chaos = direct = fire = 0;
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        public GargishEquivocation(Serial serial)
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