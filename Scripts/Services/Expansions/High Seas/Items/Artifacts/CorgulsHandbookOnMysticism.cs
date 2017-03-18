using Server;
using System;

namespace Server.Items
{
    public class CorgulsHandbookOnMysticism : MysticBook
	{
        public override int LabelNumber { get { return 1149779; } }

		[Constructable]
		public CorgulsHandbookOnMysticism ()
		{
			Hue = 1159;
			Attributes.RegenMana = 3;
			Attributes.DefendChance = 5;
			Attributes.LowerManaCost = 10;
			Attributes.LowerRegCost = 20;
		}

        public CorgulsHandbookOnMysticism(Serial serial)
            : base(serial)
		{
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