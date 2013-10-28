using System;

namespace Server.Items
{
    public class MischiefMaker : MagicalShortbow, ITokunoDyable
    {
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
            this.Hue = 0x8AB;
            this.Balanced = true;
			
            this.Slayer = SlayerName.Exorcism;
			
            this.Attributes.WeaponSpeed = 35;
            this.Attributes.WeaponDamage = 45;
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
        }
    }
}