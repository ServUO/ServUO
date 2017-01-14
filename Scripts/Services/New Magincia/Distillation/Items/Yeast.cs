using Server;
using System;
using Server.Engines.Distillation;

namespace Server.Items
{
	public class Yeast : Item
	{
		private int m_BacterialResistance;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public int BacterialResistance 
		{ 
			get { return m_BacterialResistance; } 
			set 
			{ 
				m_BacterialResistance = value; 
				
				if(m_BacterialResistance < 1) m_BacterialResistance = 1;
				if(m_BacterialResistance > 5) m_BacterialResistance = 5;
				
				InvalidateProperties(); 
			} 
		}
		
		public override int LabelNumber { get { return 1150453; } } // yeast
		
		[Constructable]
		public Yeast() : base(3624)
		{
            Hue = 2418;
			int ran = Utility.Random(100);

			if(ran <= 5)
				m_BacterialResistance = 5;
			else if (ran <= 10)
				m_BacterialResistance = 4;
			else if (ran <= 20)
				m_BacterialResistance = 3;
			else if (ran <= 40)
				m_BacterialResistance = 2;
			else
				m_BacterialResistance = 1;
		}
		
		[Constructable]
		public Yeast(int resistance) : base(3624)
		{
			BacterialResistance = resistance;
		}
		
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

            list.Add(1150455, GetResistanceLabel()); // Bacterial Resistance: ~1_VAL~
		}
		
		private string GetResistanceLabel()
		{	
			switch(m_BacterialResistance)
			{
				default:
				case 5: return "++"; 
				case 4: return "+";
				case 3: return "+-";
				case 2: return "-";
				case 1: return "--";
			}
		}
		
		public Yeast(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			
			writer.Write(m_BacterialResistance);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			m_BacterialResistance = reader.ReadInt();
		}
	}
}