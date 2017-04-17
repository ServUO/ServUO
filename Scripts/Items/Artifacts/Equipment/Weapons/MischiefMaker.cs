using System;

namespace Server.Items
{
    public class MischiefMaker : MagicalShortbow, ITokunoDyable
	{
		public override bool IsArtifact { get { return true; } }
        public override int LabelNumber
        {
            get
            {
                return 1072910;
            }
        }// Mischief Maker

        [Constructable]
        public MischiefMaker()
            : base()
        {
            Hue = 0x8AB;
            Balanced = true;
			
            Slayer = SlayerName.Silver;
			
            Attributes.WeaponSpeed = 35;
            Attributes.WeaponDamage = 45;
        }

        public MischiefMaker(Serial serial)
            : base(serial)
        {
        }

        #region Mondain's Legacy
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            pois = fire = phys = nrgy = chaos = direct = 0;
            cold = 100;
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

            if (Slayer != SlayerName.Silver)
            {
                Slayer = SlayerName.Silver;
            }
        }
    }
}