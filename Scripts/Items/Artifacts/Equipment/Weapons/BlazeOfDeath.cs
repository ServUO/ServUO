using System;

namespace Server.Items
{
    public class BlazeOfDeath : Halberd
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public BlazeOfDeath()
        {
            this.Hue = 0x501;
            this.WeaponAttributes.HitFireArea = 50;
            this.WeaponAttributes.HitFireball = 50;
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 35;
            this.WeaponAttributes.ResistFireBonus = 10;
            this.WeaponAttributes.LowerStatReq = 100;
        }

        public BlazeOfDeath(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1063486;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            fire = 50;
            phys = 50;

            cold = pois = nrgy = chaos = direct = 0;
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