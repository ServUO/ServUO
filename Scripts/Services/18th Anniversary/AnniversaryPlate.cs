using System;
using Server;

namespace Server.Items
{
	public class AnniversaryPlate : Item
	{
		public override int LabelNumber { get { return 1156149; } } // An Ornately Decorated Commemorative Plate
		
		[CommandProperty(AccessLevel.GameMaster)]
		public TextDefinition LabelType { get; set; }

        [Constructable]
        public AnniversaryPlate()
            : this(null)
        {
        }

		[Constructable]
		public AnniversaryPlate(Mobile m) : base(0x9BC8)
		{
			if(m != null && .01 > Utility.RandomDouble())
				LabelType = String.Format("{0} first adventure in Britannia!", m.Name); // No Cliloc???
			else
				LabelType = Utility.RandomMinMax(1156150, 1156157);
		}

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1156149); // An Ornately Decorated Commemorative Plate

            if (LabelType.Number > 0)
                list.Add(1062613, String.Format("#{0}", LabelType.Number.ToString()));
            else if (LabelType.String != null)
                list.Add(1062613, LabelType.String);
        }

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);
			
			if(LabelType.Number > 0)
				list.Add(LabelType.Number);
			else if (LabelType.String != null)
				list.Add(LabelType.String);
		}
		
		public AnniversaryPlate(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
			
			TextDefinition.Serialize(writer, LabelType);
		}
 
		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
			
			LabelType = TextDefinition.Deserialize(reader);
        }
	}
}