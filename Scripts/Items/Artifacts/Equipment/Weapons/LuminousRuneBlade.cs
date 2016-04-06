using System;

namespace Server.Items
{
    public class LuminousRuneBlade : RuneBlade
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LuminousRuneBlade()
        {
            this.WeaponAttributes.HitLightning = 40;
            this.WeaponAttributes.SelfRepair = 5;
            this.Attributes.NightSight = 1;
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 55;

            this.Hue = this.GetElementalDamageHue();
        }

        public LuminousRuneBlade(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072922;
            }
        }// Luminous Rune Blade
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = cold = pois = chaos = direct = 0;
            nrgy = 100;
        }

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