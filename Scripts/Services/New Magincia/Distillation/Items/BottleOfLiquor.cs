using Server;
using System;
using Server.Items;

namespace Server.Engines.Distillation
{
	public class BottleOfLiquor : BeverageBottle
	{
		private Liquor m_Liquor;
		private string m_Label;
        private bool m_IsStrong;
        private Mobile m_Distiller;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Liquor Liquor { get { return m_Liquor; } set { m_Liquor = value; InvalidateProperties(); } }
		
		[CommandProperty(AccessLevel.GameMaster)]
		public string Label { get { return m_Label; } set { m_Label = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsStrong { get { return m_IsStrong; } set { m_IsStrong = value; InvalidateProperties(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Distiller { get { return m_Distiller; } set { m_Distiller = value; InvalidateProperties(); } }

        public override bool ShowQuantity { get { return false; } }

		[Constructable]
		public BottleOfLiquor() : this(Liquor.Whiskey, null, false, null)
		{
		}
		
		[Constructable]
		public BottleOfLiquor(Liquor liquor, string label, bool isstrong, Mobile distiller) : base(BeverageType.Liquor)
		{
			Quantity = MaxQuantity;
			m_Liquor = liquor;
			m_Label = label;
            m_IsStrong = isstrong;
            m_Distiller = distiller;
		}
		
		public override void AddNameProperty(ObjectPropertyList list)
		{
			if(m_Label != null && m_Label.Length > 0)
				list.Add(1049519, m_Label); // a bottle of ~1_DRINK_NAME~
			else
				list.Add(1049519, String.Format("#{0}", DistillationSystem.GetLabel(m_Liquor, m_IsStrong))); // a bottle of ~1_DRINK_NAME~
		}
		
		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);
			
			if(m_Liquor != Liquor.None)
				list.Add(1150454, String.Format("#{0}", DistillationSystem.GetLabel(m_Liquor, m_IsStrong))); // Liquor Type: ~1_TYPE~

            if (m_Distiller != null)
                list.Add(1150679, m_Distiller.Name); // Distiller: ~1_NAME~

			list.Add( GetQuantityDescription() );
		}
		
		public BottleOfLiquor(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			
			writer.Write((int)m_Liquor);
			writer.Write(m_Label);
            writer.Write(m_Distiller);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			m_Liquor = (Liquor)reader.ReadInt();
			m_Label = reader.ReadString();
            m_Distiller = reader.ReadMobile();
		}
	}
}