using System;

namespace Server.Items
{
    public class LegendaryDetectiveoftheRoyalGuard : Boots
	{
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public LegendaryDetectiveoftheRoyalGuard()
            : base(0x170B)
        { 
            this.Hue = 902;
            
            this.Attributes.BonusInt = 2;
        }

        public LegendaryDetectiveoftheRoyalGuard(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits
        {
            get
            {
                return 150;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 150;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1094894;
            }
        }// Legendary Detective of the Royal Guard [Replica]
        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}
