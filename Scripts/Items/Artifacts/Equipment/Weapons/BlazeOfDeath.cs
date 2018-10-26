using System;

namespace Server.Items
{
    public class BlazeOfDeath : Halberd
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public BlazeOfDeath()
        {
            Hue = 0x501;
            WeaponAttributes.HitFireArea = 50;
            WeaponAttributes.HitFireball = 50;
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 35;
            WeaponAttributes.ResistFireBonus = 10;
            WeaponAttributes.LowerStatReq = 100;
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