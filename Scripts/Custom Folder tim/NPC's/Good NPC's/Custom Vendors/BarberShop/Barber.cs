using System; 
using System.Collections.Generic; 
using Server; 

namespace Server.Mobiles 
{ 
	public class Barber : BaseVendor 
	{ 
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } } 

		[Constructable]
		public Barber() : base( "the barber" ) 
		{ 
			SetSkill( SkillName.Alchemy, 80.0, 100.0 );
			SetSkill( SkillName.Magery, 90.0, 110.0 );
			SetSkill( SkillName.TasteID, 85.0, 100.0 );
		} 

		public override void InitSBInfo() 
		{ 
			m_SBInfos.Add( new SBBarber() ); 
		} 

		public Barber( Serial serial ) : base( serial ) 
		{ 
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 

			writer.Write( (int) 0 ); // version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 

			int version = reader.ReadInt(); 
		} 
	} 
}