/*
 * Created by jacquesc1. 
 * Date: 05/08/2009
 
 * Redesigned by Lokai
 * 3/23/2017
 */
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
	public class Shroudofcolor : HoodedShroudOfShadows
	{
		private int m_BaseColor;
		
        [CommandProperty(AccessLevel.GameMaster)]
        public int BaseColor { get { return m_BaseColor; } set { m_BaseColor = value; } }
		
		private string m_NameMod;
		
		[Constructable]
		public Shroudofcolor()
		{
			Name = "Shroud of Color";
            Hue = 0;
			m_BaseColor = 0;
			m_NameMod = "";
		}
		
		public override bool OnEquip(Mobile m) 
	    {
			m_NameMod = m.NameMod;
			m.NameMod = m.Name + "'s Shroud of Color";
			m.DisplayGuildTitle = false;	
			m.SendMessage( "The cloak will transform it's colour when you click on it!" );
			m.PlaySound( 484 );
			return base.OnEquip(m);
		}
        public override void OnDoubleClick(Mobile from)
        {
			if (this.IsChildOf(from.Backpack) || this == from.FindItemOnLayer(Layer.OuterTorso))
				ChangeColor(true);
			else
				from.SendMessage("You must be holding or wearing this to change it's color");
        }
		
		public void ChangeColor(bool random)
		{
			Hue = random?Utility.Random( 3000 ):m_BaseColor;
		}
		
		public override void OnRemoved( object parent) 
	    { 
			if (parent is Mobile) 
	        { 
				Mobile m = (Mobile)parent; 
				m.NameMod = m_NameMod;
				m.SendMessage( "You're back to your old self." );
				m.PlaySound( 484 );		   
				m.DisplayGuildTitle = true;
		  }

	         base.OnRemoved(parent); 
      	}

        public Shroudofcolor(Serial serial): base(serial)
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
			
			writer.Write( (int) m_BaseColor );
			writer.Write((string)m_NameMod);
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			m_BaseColor = reader.ReadInt();
			m_NameMod = reader.ReadString();
		}
	}
}