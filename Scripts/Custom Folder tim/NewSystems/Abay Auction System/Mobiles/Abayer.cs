#region AuthorHeader
//
//	Abay version 2.1, by Xanthos and Arya
//
//  Based on original ideas and code by Arya
//
#endregion AuthorHeader
using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.ContextMenus;

namespace Arya.Abay
{
	#region Context Menu

	public class TradeHouseEntry : ContextMenuEntry
	{
		private Abayer m_Abayer;
		
		 

		public TradeHouseEntry( Abayer Abayer ) : base( 6103, 10 )
		{
			m_Abayer = Abayer;
		}

		public override void OnClick()
		{
			Mobile m = Owner.From;

			if ( ! m.CheckAlive() )
				return;

			if ( AbaySystem.Running )
			{
				m.SendGump( new AbayGump( m ) );
			}
			else if ( m_Abayer != null )
			{
				m_Abayer.SayTo( m, AbaySystem.ST[ 145 ] );
			}
		}
	}

	#endregion

	/// <summary>
	/// Summary description for Abayer.
	/// </summary>
	public class Abayer : BaseVendor
	{
	
		private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();

		[ Constructable ]
		public Abayer() : base ( "the Abayer" )
		{
			RangePerception = 10;
		}
		
		protected override List<SBInfo> SBInfos { get { return m_SBInfos; } }


		public override void InitOutfit()
		{
			AddItem( new LongPants( GetRandomHue() ) );
			AddItem( new Boots( GetRandomHue() ) );
			AddItem( new FeatheredHat( GetRandomHue() ) );

			if ( Female )
			{
				AddItem( new Kilt( GetRandomHue() ) );
				AddItem( new Shirt( GetRandomHue() ) );

				switch( Utility.Random( 3 ) )
				{
					case 0: AddItem( new LongHair( GetHairHue() ) ); break;
					case 1: AddItem( new PonyTail( GetHairHue() ) ); break;
					case 2: AddItem( new BunsHair( GetHairHue() ) ); break;
				}

				GoldBracelet bracelet = new GoldBracelet();
				bracelet.Hue = GetRandomHue();
				AddItem( bracelet );

				GoldNecklace neck = new GoldNecklace();
				neck.Hue = GetRandomHue();
				AddItem( neck );
			}
			else
			{
				AddItem( new FancyShirt( GetRandomHue() ) );
				AddItem( new Doublet( GetRandomHue() ) );

				switch( Utility.Random( 2 ) )
				{
					case 0: AddItem( new PonyTail( GetHairHue() ) ); break;
					case 1: AddItem( new ShortHair( GetHairHue() ) ); break;
				}
			}
		}

		public Abayer( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize (writer);

			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize (reader);

			reader.ReadInt();
		}

		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			list.Add( new TradeHouseEntry( this ) );
		}

		public override void InitSBInfo()
		{
		}

		// protected override System.Collections.ArrayList SBInfos
		// {
			// get
			// {
				// return new ArrayList();
			// }
		// }

		public override void OnSpeech(SpeechEventArgs e)
		{
			if ( e.Speech.ToLower().IndexOf( "Abay" ) > -1 )
			{
				e.Handled = true;

				if ( ! e.Mobile.CheckAlive() )
				{
					SayTo( e.Mobile, "Am I hearing voices?" );
				}
				else if ( AbaySystem.Running )
				{
					e.Mobile.SendGump( new AbayGump( e.Mobile ) );
				}
				else
				{
					SayTo( e.Mobile, "Sorry, we're closed at this time. Please try again later." );
				}
			}
			else if ( e.Speech.ToLower().IndexOf( "version" ) > -1 )
				SayTo( e.Mobile, "Abay version 2.1, by Xanthos and Arya" );

			base.OnSpeech (e);
		}
	}
}