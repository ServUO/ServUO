using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;
using Server.Prompts;
using Server.Network;
using Server.ContextMenus;

namespace Server.Mobiles
{
	public class Dealer : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public Dealer() : base( "the Narcotics Dealer" )
		{
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBNarcotics() );
		}

		public override VendorShoeType ShoeType
		{
			get{ return VendorShoeType.Sandals; }
		}

		public override int GetShoeHue()
		{
			return 0;
		}

		public override  void InitBody() 
		{
			base.InitBody();
		}

		public override void InitOutfit()
		{
			AddItem( new Server.Items.Shirt( Utility.RandomRedHue() ) );
			AddItem( new Server.Items.SkullCap( Utility.RandomBlueHue() ) );
			AddItem( new Server.Items.ShortPants( Utility.RandomBlueHue() ) );
			int hairHue=0x3E;
			switch ( Utility.Random( 9 ) )
			{
				case 0: AddItem( new Afro( hairHue ) ); break;
				case 1: AddItem( new KrisnaHair( hairHue ) ); break;
				case 2: AddItem( new PageboyHair( hairHue ) ); break;
				case 3: AddItem( new PonyTail( hairHue ) ); break;
				case 4: AddItem( new ReceedingHair( hairHue ) ); break;
				case 5: AddItem( new TwoPigTails( hairHue ) ); break;
				case 6: AddItem( new ShortHair( hairHue ) ); break;
				case 7: AddItem( new LongHair( hairHue ) ); break;
				case 8: AddItem( new BunsHair( hairHue ) ); break;
			}
		}

		public Dealer( Serial serial ) : base( serial )
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
