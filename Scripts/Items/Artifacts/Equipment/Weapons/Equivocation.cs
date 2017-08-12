using System;

namespace Server.Items
{
    public class Equivocation : GnarledStaff
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1154473; } } // Equivocation

        [Constructable]
        public Equivocation()
        {
            this.Attributes.BalancedWeapon = 1;

            this.Slayer2 = BaseRunicTool.GetRandomSlayer();
            this.Attributes.AttackChance = 10;
            this.Attributes.RegenHits = 6;
            this.Attributes.Brittle = 1;
            this.Attributes.WeaponSpeed = 35;
            this.Attributes.WeaponDamage = 50;
            this.WeaponAttributes.HitCurse = 15;

            this.Hue = 1365;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = pois = 50;
            cold = nrgy = chaos = direct = fire = 0;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public Equivocation(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class GargishEquivocation : GargishGnarledStaff
    {
        public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1154473; } } // Equivocation

        [Constructable]
        public GargishEquivocation()
        {
            this.Attributes.BalancedWeapon = 1;

            this.Slayer2 = BaseRunicTool.GetRandomSlayer();
            this.Attributes.AttackChance = 10;
            this.Attributes.RegenHits = 6;
            this.Attributes.Brittle = 1;
            this.Attributes.WeaponSpeed = 35;
            this.Attributes.WeaponDamage = 50;
            this.WeaponAttributes.HitCurse = 15;

            this.Hue = 1365;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = pois = 50;
            cold = nrgy = chaos = direct = fire = 0;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public GargishEquivocation(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}