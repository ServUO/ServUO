using System;

namespace Server.Items
{
    public class TurqouiseRing : GoldRing
    {
        [Constructable]
        public TurqouiseRing()
            : base()
        {
            this.Weight = 1.0;

            BaseRunicTool.ApplyAttributesTo(this, true, 0, Utility.RandomMinMax(1, 3), 0, 100);
			
            if (Utility.Random(100) < 10)
                this.Attributes.WeaponSpeed += 5;	
            else
                this.Attributes.WeaponDamage += 15;
        }

        public TurqouiseRing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073460;
            }
        }// turquoise ring
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}