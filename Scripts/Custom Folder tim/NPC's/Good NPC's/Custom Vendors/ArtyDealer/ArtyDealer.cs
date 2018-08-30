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
	public class ArtyDealer : BaseVendor
	{
		private List<SBInfo> m_SBInfos = new List<SBInfo>();
		protected override List<SBInfo> SBInfos{ get { return m_SBInfos; } }

		[Constructable]
		public ArtyDealer() : base( "the Artifact Dealer" )
		{
		}

		public override void InitSBInfo()
		{
			m_SBInfos.Add( new SBArtifact() );
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
			//AddItem( new Server.Items.HatOfTheMagi( Utility.RandomRedHue() ) );
			AddItem( new Server.Items.HoodedShroudOfShadows( Utility.RandomBlueHue() ) );
			AddItem( new Server.Items.Sandals( Utility.RandomBlueHue() ) );
			//AddItem( new Server.Items.InquisitorsResolution( Utility.RandomRedHue() ) );
			int hairHue=0x494;
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

		public ArtyDealer( Serial serial ) : base( serial )
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
