using System;

namespace Server.Items
{
    public class GargishEquivocation : GargishGnarledStaff
    {
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1154473; } } // Equivocation

        [Constructable]
        public GargishEquivocation()
        {
            // TODO attributes Balanced
            this.Slayer2 = BaseRunicTool.GetRandomSlayer();
            this.Attributes.AttackChance = 10;
            this.Attributes.RegenHits = 6;
            this.Attributes.Brittle = 1;
            this.Attributes.WeaponSpeed = 35;
            this.Attributes.WeaponDamage = 50;
            this.WeaponAttributes.HitCurse = 15;

            this.Hue = 2124;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = pois = 50;
            cold = nrgy = chaos = direct = fire = 0;
        }

        public override int InitMinHits { get { return 150; } }
        public override int InitMaxHits { get { return 150; } }

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