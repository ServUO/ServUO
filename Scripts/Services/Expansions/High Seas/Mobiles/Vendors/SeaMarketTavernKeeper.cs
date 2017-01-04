using System; 
using System.Collections.Generic; 
using Server; 

namespace Server.Mobiles 
{ 
	public class SeaMarketTavernKeeper : BaseVendor 
	{ 
		private List<SBInfo> m_SBInfos = new List<SBInfo>(); 
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } } 

		[Constructable]
		public SeaMarketTavernKeeper() : base( "the tavern keeper" ) 
		{ 
		} 

		public override void InitSBInfo() 
		{
            m_SBInfos.Add(new SBSeaMarketTavernKeeper()); 
		}

		public override void InitOutfit()
		{
			base.InitOutfit();

			AddItem( new Server.Items.HalfApron() );
		}

		public SeaMarketTavernKeeper( Serial serial ) : base( serial ) 
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