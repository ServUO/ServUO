using System;

namespace Server.Items
{
    public class FaerieFire : ElvenCompositeLongbow, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber
        {
            get
            {
                return 1072908;
            }
        }// Faerie Fire

        [Constructable]
        public FaerieFire()
            : base()
        {
            this.Hue = 0x489;
            this.Balanced = true;
			
            this.Attributes.BonusDex = 3;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = 60;
			
            this.WeaponAttributes.HitFireball = 25;
        }

        public FaerieFire(Serial serial)
            : base(serial)
        {
        }

        #region Mondain's Legacy
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = cold = pois = nrgy = chaos = direct = 0;
            fire = 100;
        }

        #endregion

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