using System;

namespace Server.Items
{
    public class WhitePearlBracelet : GoldBracelet
    {
        [Constructable]
        public WhitePearlBracelet()
            : base()
        {
            this.Weight = 1.0;
			
            this.Attributes.NightSight = 1;
			
            BaseRunicTool.ApplyAttributesTo(this, Utility.RandomMinMax(3, 5), 0, 100);
			
            if (Utility.Random(100) < 50)
            {
                switch ( Utility.Random(3) )
                {
                    case 0:
                        this.Attributes.CastSpeed += 1;
                        break;
                    case 1:
                        this.Attributes.CastRecovery += 2;
                        break;
                    case 2:
                        this.Attributes.LowerRegCost += 10;
                        break;
                }
            }
        }

        public WhitePearlBracelet(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073456;
            }
        }// white pearl bracelet
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