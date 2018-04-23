// Created by GreyWolf.
// Created Oct. 28, 2007

using System;
using Server.Misc;

namespace Server.Items
{
	[FlipableAttribute( 0x170d, 0x170e )] 
	public class SandalsoftheTamer : Sandals
	{
        private SkillMod m_SkillMod0;
        private SkillMod m_SkillMod1;

		
		[Constructable] 
		public SandalsoftheTamer() : base( 0x170d ) 
		{ 
			Name = "The Sandals of the Tamer";
			Hue = 0x481;

			DefineMods();
		} 

		private void DefineMods()
		{
            m_SkillMod0 = new DefaultSkillMod(SkillName.AnimalTaming, true, 10);
            m_SkillMod1 = new DefaultSkillMod(SkillName.AnimalLore, true, 10);

		}

		private void SetMods( Mobile wearer )
		{
            wearer.AddSkillMod(m_SkillMod0);
            wearer.AddSkillMod(m_SkillMod1);

		}

		public override bool OnEquip( Mobile from ) 
		{ 
			SetMods( from );
			return true;  
		} 

		public override void OnRemoved( object parent ) 
		{ 
			if ( parent is Mobile ) 
			{ 
				Mobile m = (Mobile)parent;
				m.RemoveStatMod( "SandalsoftheTamer" );

                if (m_SkillMod0 != null)
                    m_SkillMod0.Remove();

                if (m_SkillMod1 != null)
                    m_SkillMod1.Remove();

			} 
		} 

		public override void OnSingleClick( Mobile from ) 
		{ 
			this.LabelTo( from, Name ); 
		}

        public SandalsoftheTamer(Serial serial): base(serial) 
		{ 
			DefineMods();
			
			if ( Parent != null && this.Parent is Mobile ) 
				SetMods( (Mobile)Parent );
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); 
		} 

		public override void Deserialize(GenericReader reader) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
		} 
	} 
} 
