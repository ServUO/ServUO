using Server;
using System;

namespace Server.Items
{
    public class YellowPolkaDotBikini : LeatherBustierArms
	{
        public override int LabelNumber { get { return 1149962; } }

		[Constructable]
		public YellowPolkaDotBikini() 
		{
            Hue = 1169;
		}

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add(1041645); // recovered from a shipwrecklist
        }

        public YellowPolkaDotBikini(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{	
			base.Serialize(writer);
            writer.Write((int)1);	
		}
	
		public override void Deserialize(GenericReader reader)
		{
		 	base.Deserialize(reader);
			int version = reader.ReadInt();

            if (version == 0)
                Hue = 1169;
		}
	}
}