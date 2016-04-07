using System;

namespace Server.Items
{
    public class ColdBlood : Cleaver
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public ColdBlood()
        {
            this.Hue = 0x4F2;

            this.Attributes.WeaponSpeed = 40;

            this.Attributes.BonusHits = 6;
            this.Attributes.BonusStam = 6;
            this.Attributes.BonusMana = 6;
        }

        public ColdBlood(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070818;
            }
        }// Cold Blood
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
            cold = 100;

            fire = phys = pois = nrgy = chaos = direct = 0;
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