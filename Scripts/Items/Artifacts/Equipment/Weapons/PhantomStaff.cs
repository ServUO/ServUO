using System;

namespace Server.Items
{
    public class PhantomStaff : WildStaff
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public PhantomStaff()
        {
            this.Hue = 0x1;
            this.Attributes.RegenHits = 2;
            this.Attributes.NightSight = 1;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = 60;
        }

        public PhantomStaff(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072919;
            }
        }// Phantom Staff
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = nrgy = chaos = direct = 0;
            cold = pois = 50;
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