using System;

namespace Server.Items
{
    public class Abhorrence : Crossbow
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber { get { return 1154472; } } // Abhorrence

        [Constructable]
        public Abhorrence()
        {
            this.Attributes.SpellChanneling = 1;
            this.WeaponAttributes.HitLightning = 35;
            this.WeaponAttributes.HitLeechMana = 40;
            this.WeaponAttributes.HitLowerDefend = 20;
            this.Attributes.WeaponSpeed = 35;
            this.Attributes.WeaponDamage = 50;
            this.MaxRange = 8;

            ExtendedWeaponAttributes.Bane = 1;

            this.Hue = 1910; // checked
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            nrgy = 100;
            phys = pois = cold = chaos = direct = fire = 0;
        }

        public override int InitMinHits { get { return 255; } }
        public override int InitMaxHits { get { return 255; } }

        public Abhorrence(Serial serial)
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