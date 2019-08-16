using System;

namespace Server.Items
{   
    public class MurkyMilk : Pitcher
    {
		public override int LabelNumber {get {return 1153874;} } // Murky Milk
		public override int MaxQuantity { get { return 5; } }
        public override double DefaultWeight { get { return 1; } }
		
        [Constructable]
        public MurkyMilk()
            : base(BeverageType.Milk)
        {
            Hue = 0x3e5;
            Quantity = this.MaxQuantity;
            ItemID = (Utility.RandomBool()) ? 0x09F0 : 0x09AD;
        }

        public MurkyMilk(Serial serial)
            : base(serial)
        {
        }
              
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