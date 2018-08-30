using System;
using System.Text;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

/*
 * Al@02/09/2006: Various fixes:
 *      -Display inscription without crashing the server.
 *      -(De-)serialization of member variables so that spouse and inscription
 *          survives server restart. Version changed from 0 to 1.
 *      -Only display spouse and inscription if the name has not been changed
 *          so that rings inscribed by GMs do not display nonsense.
 */

namespace Server.Items
{
	public class WeddingDeed : Item
	{
		[Constructable]
		public WeddingDeed() : base( 0x2258 )
		{
			Weight = 1.0;
			Name = "a wedding deed";
			LootType = LootType.Blessed;
		}

		public WeddingDeed( Serial serial ) : base( serial )
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

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.InRange( this.GetWorldLocation(), 1 ) )
			{
				from.CloseGump( typeof( WeddingDeedGump ) );
				from.SendGump( new WeddingDeedGump( this ) );
			}
			else
			{
				from.LocalOverheadMessage( MessageType.Regular, 906, 1019045 ); // I can't reach that.
			}
		}

		public class WeddingDeedGump : Gump
		{
			private WeddingDeed m_WeddingDeed;

			public static void Initialize()
			{
			}


			public WeddingDeedGump( WeddingDeed deed ) : base( 0,0 )
			{
				m_WeddingDeed = deed;
				AddBackground( 25, 25, 590, 455, 2600 );

				AddLabel( 130, 410, 1152, "Note: Wedding Deed has one use only, even if you are turned down." );

				AddButton( 385, 435, 0xFB1, 0xFB3, 27, GumpButtonType.Reply, 0 );
				AddLabel( 420, 435, 1152, "Exit" );
				AddButton( 185, 435, 4005, 4007, 28, GumpButtonType.Reply, 0 );
				AddLabel( 220, 435, 1152, "Propose Marriage" );

	         		AddHtml( 70, 45, 500, 27,"<center>Wedding Deed</center>", true, false );
        	 		AddItem( 100, 85, 0x2258 );
         			AddItem( 500, 85, 0x2258 );
         			AddLabel( 165, 75, 1152, "Congratulations, you are about to propose marriage" );
	         		AddLabel( 165, 90, 1152, "to the person you love. Remember that marriage is" );
        	 		AddLabel( 165, 105, 1152, "for life, and only through true death you can part." );

				AddLabel( 200, 125, 1152, "Select Wedding Ring inscription:" );

				AddRadio( 70, 150, 210, 211, true, 1 );
				AddLabel( 105, 150, 1152, "All my love, all my life");

				AddRadio( 70, 175, 210, 211, false, 2 );
				AddLabel( 105, 175, 1152, "Faithful Love Will Ever Last");

				AddRadio( 70, 200, 210, 211, false, 3 );
				AddLabel( 105, 200, 1152, "Forever In My Heart");

				AddRadio( 70, 225, 210, 211, false, 4 );
				AddLabel( 105, 225, 1152, "From This Day On");

				AddRadio( 70, 250, 210, 211, false, 5 );
				AddLabel( 105, 250, 1152, "Grow Old With Me");

				AddRadio( 70, 275, 210, 211, false, 6 );
				AddLabel( 105, 275, 1152, "How Do I Love Thee");

				AddRadio( 70, 300, 210, 211, false, 7 );
				AddLabel( 105, 300, 1152, "I Will Never Love Another");

				AddRadio( 70, 325, 210, 211, false, 8 );
				AddLabel( 105, 325, 1152, "Love Conquers All Things");

				AddRadio( 70, 350, 210, 211, false, 9 );
				AddLabel( 105, 350, 1152, "Love Me And Leave Me Not");

				AddRadio( 70, 375, 210, 211, false, 10 );
				AddLabel( 105, 375, 1152, "Love, Faith, Hope Together");

				AddRadio( 300, 150, 210, 211, false, 11 );
				AddLabel( 335, 150, 1152, "My Heart Is Yours Forever");

				AddRadio( 300, 175, 210, 211, false, 12 );
				AddLabel( 335, 175, 1152, "My Love Is Forever Yours");

				AddRadio( 300, 200, 210, 211, false, 13 );
				AddLabel( 335, 200, 1152, "Never Another You");

				AddRadio( 300, 225, 210, 211, false, 14 );
				AddLabel( 335, 225, 1152, "One Love, One Lifetime");

				AddRadio( 300, 250, 210, 211, false, 15 );
				AddLabel( 335, 250, 1152, "Today-Tomorrow-Forever");

				AddRadio( 300, 275, 210, 211, false, 16 );
				AddLabel( 335, 275, 1152, "Undying devotion");

				AddRadio( 300, 300, 210, 211, false, 17 );
				AddLabel( 335, 300, 1152, "With All That I Am");

				AddRadio( 300, 325, 210, 211, false, 18 );
				AddLabel( 335, 325, 1152, "You And No Other");

				AddRadio( 300, 350, 210, 211, false, 19 );
				AddLabel( 335, 350, 1152, "You Roxxorz My Heart");

				AddRadio( 300, 375, 210, 211, false, 20 );
				AddLabel( 335, 375, 1152, "You Shall Never Walk Alone");

			}

			public override void OnResponse( NetState state, RelayInfo info ) //Function for GumpButtonType.Reply Buttons
			{

				if ( m_WeddingDeed.Deleted )
					return;
				if ( info.ButtonID == 28 )
				{

					Mobile from = state.Mobile;

					if ( !m_WeddingDeed.IsChildOf( from.Backpack ) )
					{
						from.SendLocalizedMessage( 1042010 ); //You must have the objectin your backpack to use it.
						return;
					}

					for (int i = 1; i < 21 ; i++)
					{
						if( info.IsSwitched(i) )
						{
							from.Target = new WeddingTarget( i, m_WeddingDeed );
						}
					}
				}
			}
		}

		public class WeddingTarget : Target
		{
			private int inscription;
			private WeddingDeed WeddingDeed;

			public WeddingTarget( int i, WeddingDeed deed ) : base( 3, false, TargetFlags.None )
			{
				inscription = i;
				WeddingDeed = deed;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				string title;

				if ( o is PlayerMobile )
				{
					Mobile to = o as Mobile;
					if ( from.Female && to.Female)
						title = "lesbian wife";
					else if (from.Female && !to.Female)
						title = "husband";
					else if (!from.Female && to.Female)
						title = "wife";
					else
						title = "gay husband";

					from.Say( "I " + from.Name + " humbly ask you " + to.Name + " to be my " + title + "." );

					from.SendGump( new WaitGump( to ) );
					to.SendGump( new ProposalGump( inscription, WeddingDeed, from ) );


				}
				else
				{
					from.SendMessage("You cannot marry that" );
				}

			}

		}

		public class WaitGump : Gump
		{
			private Mobile to;
			public static void Initialize()
			{
			}

			public WaitGump(Mobile m) : base ( 0, 0)
			{
				to = m;
				AddBackground( 150, 300, 320, 200, 2600 );
         			AddLabel( 200, 350, 1152, "You have asked your true love to" );
	         		AddLabel( 200, 370, 1152, "marry you. Now all there is left" );
        	 		AddLabel( 200, 390, 1152, "is to wait for an answer. Good Luck" );
				    AddButton( 315, 460, 0xFB1, 0xFB3, 1, GumpButtonType.Reply, 0 );
        	 		AddLabel( 350, 460, 1152, "Cancle" );
			}

			public override void OnResponse( NetState state, RelayInfo info ) //Function for GumpButtonType.Reply Buttons
			{
				Mobile from = state.Mobile;

				from.CloseGump( typeof( WaitGump ) );
				to.CloseGump( typeof( ProposalGump ) );
			}

		}

		public class ProposalGump : Gump
		{
			private WeddingDeed WeddingDeed;
			private int inscription;
			private Mobile from;

			public ProposalGump( int i, WeddingDeed deed, Mobile m ) : base ( 0, 0)
			{
				inscription = i;
				WeddingDeed = deed;
				from = m;

				AddBackground( 150, 300, 320, 200, 2600 );
         			AddLabel( 200, 350, 1152, "Somebody has asked for your hand in" );
	         		AddLabel( 200, 370, 1152, "marriage. If you chose to accept," );
        	 		AddLabel( 200, 390, 1152, "then you will be married for life." );
        	 		AddButton( 215, 460, 4005, 4007, 1, GumpButtonType.Reply, 0 );
        	 		AddLabel( 250, 460, 1152, "Accept" );
        	 		AddButton( 315, 460, 0xFB1, 0xFB3, 2, GumpButtonType.Reply, 0 );
        	 		AddLabel( 350, 460, 1152, "Decline" );


			}

			public override void OnResponse( NetState state, RelayInfo info ) //Function for GumpButtonType.Reply Buttons
			{
				Mobile to = state.Mobile;

				if ( info.ButtonID == 1)
				{
					from.CloseGump( typeof( WaitGump ) );
					to.CloseGump( typeof( ProposalGump ) );
					Item gRing = from.FindItemOnLayer( Layer.Ring );
					Item rRing = to.FindItemOnLayer( Layer.Ring );
					if( gRing == null && rRing == null)
					{
						//Marriage can proceed
						to.Say( "Oh yes! " + from.Name + " of course I will marry you.");
						WeddingDeed.Delete();
						WeddingRing newring1 = new WeddingRing( inscription, to.Name );
						WeddingRing newring2 = new WeddingRing( inscription, from.Name );
						from.EquipItem( newring1 );
						to.EquipItem( newring2 );
						from.PlaySound( 0x40b );
						World.Broadcast( 0x481, true, "{0} and {1} have just been married. Congratulations!", from.Name, to.Name );
					}
					else if(gRing is WeddingRing)
					{
						from.SendMessage("You are already married!");
						to.SendMessage("The other person is already maried!");
					}
					else if(rRing is WeddingRing)
					{
						to.SendMessage("You are already married!");
						from.SendMessage("The other person is already maried!");
					}
					else
					{
						to.SendMessage("The marriage cannont proceed because one of you is wearing a ring.");
						from.SendMessage("The marriage cannont proceed because one of you is wearing a ring.");
					}



				}
				else if (info.ButtonID == 2)
				{
					to.Say( "I am sorry " + from.Name + " but I cannot marry you.");
					WeddingDeed.Delete();
					from.SendMessage("Alas, your proposal was declined and you lost your deed.");
					from.CloseGump( typeof( WaitGump ) );
					to.CloseGump( typeof( ProposalGump ) );
				}
				else
				{
					from.CloseGump( typeof( WaitGump ) );
					to.CloseGump( typeof( ProposalGump ) );
				}



			}
		}

		public class WeddingRing : BaseRing
		{

			private int index;
			private static string[] inscr = {
                        "",
                        "All my love, all my life",
						"Faithful Love Will Ever Last",
						"Forever In My Heart",
						"From This Day On",
						"Grow Old With Me",
						"How Do I Love Thee",
						"I Will Never Love Another",
						"Love Conquers All Things",
						"Love Me And Leave Me Not",
						"Love, Faith, Hope Together",
						"My Heart Is Yours Forever",
						"My Love Is Forever Yours",
						"Never Another You",
						"One Love, One Lifetime",
						"Today-Tomorrow-Forever",
						"Undying devotion",
						"With All That I Am",
						"You And No Other",
						"You Roxxorz My Heart",
						"You Shall Never Walk Alone"
            };
			private string spouse;

			[CommandProperty( AccessLevel.Seer )]
			public string Spouse{ get{ return spouse; } set{ spouse = value; } }

			[CommandProperty( AccessLevel.Seer )]
			public int InscriptionIndex{ get{ return index; } set{ index = value; } }

			[Constructable]
			public WeddingRing(int i, string s ) : base( 0x108a )
			{

				Weight = 0.1;
				Hue = 1153;
				Name = "Wedding Ring - Spouse: ";
				Movable = true;
				LootType = LootType.Blessed;

				spouse = s;
				if( i >= 1 && i <= 20)
					index = i;
				else
					index = 1;
			}

			public WeddingRing( Serial serial ) : base( serial )
			{
			}

			public override void OnSingleClick( Mobile from )
			{
				base.OnSingleClick( from );
                if (Name == "Wedding Ring - Spouse: ")
                //Ensure the name has not been changed by a GM.
                {
                    base.LabelTo(from, "{0}", spouse);
                    base.LabelTo(from, "{0}", inscr[index]);
                }
			}


			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 1 ); // version al: set version to 1
                writer.Write(index);
                writer.Write(spouse);
            }

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );
				int version = reader.ReadInt();
                if (version == 1)
                {
                    index = reader.ReadInt();
                    spouse = reader.ReadString();
                }
            }
		}

	}
}