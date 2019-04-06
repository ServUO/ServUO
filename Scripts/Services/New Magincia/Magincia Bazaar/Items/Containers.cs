using System;
using Server;
using Server.Items;

namespace Server.Engines.NewMagincia
{
	public class WarehouseContainer : MediumCrate
	{
		private Mobile m_Owner;
		
		[CommandProperty(AccessLevel.GameMaster)]
		public Mobile Owner { get { return m_Owner; } }
		
		public WarehouseContainer(Mobile owner)
		{
			m_Owner = owner;
		}
		
		public WarehouseContainer(Serial serial) : base(serial)
		{
		}
		
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0);
			
			writer.Write(m_Owner);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			
			m_Owner = reader.ReadMobile();
		}
	}
	
	public class Warehouse : LargeCrate
	{
		public Warehouse(Mobile owner)
		{
			Movable = false;
			Visible = false;
		}
		
		public Warehouse(Serial serial) : base(serial)
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
