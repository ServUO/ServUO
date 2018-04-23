// PlayingCards by zulu updated February 2004
// some bug fixes/updates XxSP1DERxX
using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Gumps;
using System.Collections;
using Server.Mobiles;

namespace Server.Mobiles
{

    public class Gambler : BaseCreature
    {
	public static bool NewCards = false;

	private int m_current_card = 53;
	private int [] Cardz = new int[53];
	private int [] dealercards = new int[5];
	private int [] playercards = new int[5];
	private int [] gamestats = new int[6]; // win-loss-tie
	private bool pbj = false;
	private bool dbj = false;

	private int playerbet = 100;
	private bool roundend;
	private bool dealercardhidden;
	private bool busy;
	private string pokermsg ="";
	private int dwin = 0;
	private int pwin = 0;
	private Mobile m_player;

        [Constructable]
        public Gambler() : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.8, 3.0 ) 
        {
            SetStr( 10, 30 );
            SetDex( 10, 30 );
            SetInt( 10, 30 );
            Fame = 50;
            Karma = 50;
            SpeechHue = Utility.RandomDyedHue();
            Title = "the gambler";
            Hue = Utility.RandomSkinHue();
            Blessed = true;
            NameHue = 0x35;

            if ( this.Female = Utility.RandomBool() )
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName( "female" );
                Item hair = new Item( Utility.RandomList( 0x203B, 0x203C, 0x203D, 0x2045, 0x204A, 0x2046 , 0x2049 ) );
                hair.Hue = Utility.RandomHairHue();
                hair.Layer = Layer.Hair;
                hair.Movable = false;
                AddItem( hair );
                Item hat = null;
                switch ( Utility.Random( 5 ) )
                {
                    case 0: hat = new FloppyHat( Utility.RandomNeutralHue() );		break;
                    case 1: hat = new FeatheredHat( Utility.RandomNeutralHue() );	break;
                    case 2: hat = new Bonnet();						break;
                    case 3: hat = new Cap( Utility.RandomNeutralHue() );		break;
                }
	    	AddItem( hat );
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName( "male" );
                Item hair = new Item( Utility.RandomList( 0x203B, 0x203C, 0x203D, 0x2044, 0x2045, 0x2047, 0x2048 ) );
                hair.Hue = Utility.RandomHairHue();
                hair.Layer = Layer.Hair;
                hair.Movable = false;
                AddItem( hair );
                Item beard = new Item( Utility.RandomList( 0x0000, 0x203E, 0x203F, 0x2040, 0x2041, 0x2067, 0x2068, 0x2069 ) );
                beard.Hue = hair.Hue;
                beard.Layer = Layer.FacialHair;
                beard.Movable = false;
                AddItem( beard );
                Item hat = null;
                switch ( Utility.Random( 7 ) )
                {
                    case 0: hat = new SkullCap( GetRandomHue() );			break;
                    case 1: hat = new Bandana( GetRandomHue() );			break;
                    case 2: hat = new WideBrimHat();					break;
                    case 3: hat = new TallStrawHat( Utility.RandomNeutralHue() );	break;
                    case 4: hat = new StrawHat( Utility.RandomNeutralHue() );		break;
                    case 5: hat = new TricorneHat( Utility.RandomNeutralHue() );	break;
                }
	    	AddItem( hat );
	    }
            AddItem( new LongPants( GetRandomHue() ) );
            AddItem( new FancyShirt( GetRandomHue() ) );
            AddItem( new Boots( Utility.RandomNeutralHue() ) );
            AddItem( new Cloak( GetRandomHue() ) );
            AddItem( new BodySash( GetRandomHue() ) );

            Container pack = new Backpack();

            pack.DropItem( new Gold( 5, 500 ) );

            pack.Movable = false;
	    pack.Visible = false;

            AddItem( pack );

	    //reset stats
	    for ( int i = 0; i <= 5; ++i )
		gamestats[i]=0;
        }

       public override bool ClickTitle{ get{ return false; } }


       public override bool HandlesOnSpeech( Mobile from )
       {
         if ( from.InRange( this.Location, 4 ) )
            return true;

         return base.HandlesOnSpeech( from );
       }

		public override void OnSpeech( SpeechEventArgs e )
		{
			base.OnSpeech( e );

			Mobile from = e.Mobile;
			string message;

			if ( from.InRange( this, 4 ))
			{

				if (m_player != null)
				{
				if ( m_player.NetState == null )
				    busy = false;
				}
				if (e.Speech.ToLower() == "hello" || e.Speech.ToLower() == "hi" || e.Speech.ToLower() == "gambler")
				{
                    			message = "Let's play blackjack or poker.";
                    			this.Say( message );
				}
				else if (e.Speech.ToLower() == "play")
				{
                    			message = "Would you like to play blackjack or poker?";
                    			this.Say( message );
				}
				else if (e.Speech.ToLower() == "reset")
				{
                    			if ( from.AccessLevel >= AccessLevel.Seer )
                    			{
                    				busy = false;
                    				message = "I am no longer busy.";
                    				this.Say( message );
                    			}
				}
				else if  (e.Speech.ToLower() == "blackjack" || e.Speech.ToLower() == "play Blackjack")
				{
					if (!busy)
					{
					playerbet = 100;
					busy = true;
					roundend = true;
					m_current_card = 53;
					dealercardhidden = false;
					dwin = 0;
					pwin = 0;

					dealercards[0]=12;
					playercards[0]=13;
					dealercards[1]=11;
					playercards[1]=26;

					for ( int i = 2; i <= 4; ++i )
					{
						dealercards[i]=0;
						playercards[i]=0;
					}
					pokermsg = "ATA- rules";

					message = "So, you want to try your luck.";
					this.Say( message );

					m_player = from;
					playblackjack( from );
					}
					else if ( m_player.NetState == from.NetState )
					{
					message = "We are already playing cards.";
					this.Say( message );
					}
					else
					{
					message = "I am busy playing cards.";
					this.Say( message );
					}
				}
			
				else if  (e.Speech.ToLower() == "poker" || e.Speech.ToLower() == "play Poker")
				{
					if (!busy)
					{
					playerbet = 100;
					busy = true;
					roundend = true;
					m_current_card = 53;
					pwin = 0;
					dwin = 0;
					for ( int i = 0; i <= 4; ++i  )
						playercards[i]=35+i;

					pokermsg = "ATA- rules";

					m_player = from;
					playpoker( from );
					message = "So, you want to try your luck.";
					this.Say( message );
					}
					else if ( m_player.NetState == from.NetState )
					{
					message = "We are already playing cards.";
					this.Say( message );
					}
					else
					{
					message = "I am busy playing cards.";
					this.Say( message );
					}

				}
			}
		} //OnSpeech

       public void payplayer( Mobile from, int quantity)
       {
		from.AddToBackpack( new Gold( quantity ) );       	
       }

        public bool paydealer( Mobile from, int quantity)
        {
         	return from.Backpack.ConsumeTotal( typeof( Gold ), quantity );
        }

      public string CardSuit( int card )
      {
	if (card>=1 && card<=13)
   		return "C";
	else if (card>=14 && card <=26)
   		return "D";
	else if (card>=27 && card <=39)
   		return "H";
	else
   		return "S";
      }

      public string CardName( int card )
      {
	while (card>13)
	card -= 13;

	if(card==1)
   		return "A";
	else if(card == 11)
   		return "J";
	else if(card == 12)
   		return "Q";
	else if(card == 13)
   		return "K";
	else
   		return "" + card;
      }

      public int CardValue( int card )
      {
	while (card>13)
	card -= 13;

	if(card==1)
  		return 11;

	if(card>10)
 		return 10;

	return card;
      }

      public int cardcolor( string cardtemp )
      {
	if ( cardtemp == "D" || cardtemp == "H" )
	    return 32;

	return 0;
      }

      public int CardValue2( int card )
      {
	while (card>13)
	card -= 13;

	if(card==1)
  		return 14;

	return card;
	}

      public void ShuffleCards( )
      {
	int i, tempcard , tempcard2;

	for ( i = 1; i < 53; ++i )
		Cardz[i]=i;

	for ( i = 52; i >= 1; --i )
	{
	tempcard = Utility.Random( i )+1;
	tempcard2 = Cardz[tempcard];
	Cardz[tempcard] = Cardz[i];
	Cardz[i] = tempcard2;
	}
      	m_current_card = 1;
	}

	public int pickcard(Mobile from)
	{
		if (m_current_card == 53)
		{
			Effects.PlaySound( from.Location, from.Map, 0x3D );
			ShuffleCards( );
		}

		return Cardz[m_current_card++];
	}

	public void playblackjack( Mobile from )
	{
		from.SendGump( new BlackjackGump( this, this ) );
	}

	public void playpoker( Mobile from )
	{
		from.SendGump( new PokerGump( this, this ) );
	}

	public override bool DisallowAllMoves
	{
	get { return true; }
	}

	public override void OnDoubleClick( Mobile from )
	{
	if ( from.AccessLevel >= AccessLevel.Seer )
		from.SendGump( new GamblerStatsGump( this ) );
	else
		base.OnDoubleClick( from );
	}

	public Gambler( Serial serial ) : base( serial )
	{
	}

	private int GetRandomHue()
	{
            switch ( Utility.Random( 6 ) )
            {
                default:
                case 0: return 0;
                case 1: return Utility.RandomBlueHue();
                case 2: return Utility.RandomGreenHue();
                case 3: return Utility.RandomRedHue();
                case 4: return Utility.RandomYellowHue();
                case 5: return Utility.RandomNeutralHue();
            }
        }

	public class GamblerStatsGump : Gump
	{

	private Gambler m_From;

		public GamblerStatsGump( Gambler gambler ) : base( 10, 10 )
		{
			m_From = gambler;

			AddPage( 0 );

			AddBackground( 30, 100, 90, 160, 5120 );

			AddLabel( 45, 100, 70, "Blackjack" );
			AddLabel( 45, 115, 600, "Wins: "+m_From.gamestats[0] );
			AddLabel( 45, 130, 600, "Loss: "+m_From.gamestats[1] );
			AddLabel( 45, 145, 600, "Tied: "+m_From.gamestats[2] );

			AddLabel( 45, 165, 70, "Poker" );
			AddLabel( 45, 180, 600, "Wins: "+m_From.gamestats[3] );
			AddLabel( 45, 195, 600, "Loss: "+m_From.gamestats[4] );
			AddLabel( 45, 210, 600, "Tied: "+m_From.gamestats[5] );

			AddLabel(  45, 230, 1500, "Reset" );

			AddButton( 85, 235, 2117, 2118, 101, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{

			switch ( info.ButtonID )
			{
				case 101:
				{ // reset
					for ( int i = 0; i <= 5; ++i )
						m_From.gamestats[i]=0;
					break;
				}
			}
		}
	}

	public class PokerGump : Gump
	{

		private Gambler m_From;

		public PokerGump( Mobile mobile, Gambler gambler ) : base( 10, 10 )
		{
			m_From = gambler;

			int i,temp=0;
			string cardtemp="Player:";

			Closable = false;

			AddPage( 0 );

			AddImageTiled( 30, 100, 460, 160, 2624 );
			AddAlphaRegion( 90, 100, 460, 105 );

			if (m_From.dwin==2||m_From.dwin==1)
				cardtemp="Player: 1";

			AddLabel( 35, 109, 600, cardtemp );
			AddButton( 33, 243, 3, 4, 666, GumpButtonType.Reply, 0 );
			if (m_From.pwin>0)
				AddLabel( 45, 129, 70, ""+m_From.pwin );

			//show player cards
			for ( i = 0; i <= 4; ++i )
			{
				if (m_From.dwin==1)
					m_From.playercards[i]=m_From.pickcard(mobile);

				temp = m_From.playercards[i];
				if (temp>0)
				{
					if (!NewCards)
					{
					    AddBackground( 65 + ((i+1)*40), 108, 35, 50, 2171 );

					    cardtemp = m_From.CardSuit( temp );
					    AddLabel( 80 + ((i+1)*40), 134, m_From.cardcolor( cardtemp ), cardtemp );
					    AddLabel( 72 + ((i+1)*40), 113, 600, m_From.CardName( temp ) );
					    AddLabel( 65 + ((i+1)*40), 180, 500, "redeal" );
					}

					if (m_From.dwin==1||m_From.dwin==2)
					{
					    if (NewCards)
					        AddCheck( 25 + ((i+1)*75), 105, 4095+temp, 4154, false, (i+1) );
					    else
					        AddCheck( 74 + ((i+1)*40), 162, 210, 211, false, (i+1) );
					}
					else
					{
					    if (NewCards)
					    	AddImage ( 25 + ((i+1)*75), 105, 4095+temp);
					    else
					    	AddImage( 74 + ((i+1)*40), 162, 210 );
					}
				}
			}

			AddLabel(  240, 205, 800, "Deal" );
			AddButton(  220, 208, 2117, 2118, 101, GumpButtonType.Reply, 0 );

			AddLabel( 160, 205, 800, ""+m_From.playerbet );
			AddButton(  140, 208, 2117, 2118, 105, GumpButtonType.Reply, 0 );

			AddLabel( 130, 230, 64, m_From.pokermsg );

			if (m_From.dwin==1)
				m_From.dwin=2;

			if (m_From.dwin==3)
			{
				m_From.dwin=0;
				m_From.roundend = true;
			}
		}


		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;
			int i;

			switch ( info.ButtonID )
			{
				case 101:
				{  //deal
					m_From.pokermsg = "ATA- rules";

					if (!from.InRange( m_From.Location, 4 ))
					{
						m_From.roundend = true;
						m_From.busy = false;
					}
					else
					{
						if (m_From.dwin==0)
						{
						   if (m_From.paydealer( from, m_From.playerbet))
						   {
							if ((m_From.m_current_card + 10) > 52)
							{
								Effects.PlaySound( from.Location, from.Map, 0x3D );
								m_From.ShuffleCards();
							}

							for ( i = 0; i <= 4; ++i  )
								m_From.playercards[i]=0;
							m_From.dwin=1;
							m_From.roundend = false;
							m_From.pokermsg = "Click on the cards you want re-dealt.";
						   }
						   else
						   {
						   	m_From.pokermsg = "You need more money!";
						   }
						}
						else if (m_From.dwin==2)
						{
							m_From.dwin=3;

						    ArrayList Selections = new ArrayList( info.Switches );

							for ( i = 0; i <= 4; ++i  )
							{
							    if (Selections.Contains( i+1 ) != false )
							    	m_From.playercards[i]=m_From.pickcard(from);
                        			    	}
							finishpokergame(from);
						}
					}
					from.SendGump( new PokerGump( from, m_From ) );
					break;
				}
				case 105:
				{ // bet
					if (m_From.roundend)
					{
					m_From.playerbet += 100;
					if (m_From.playerbet > 500)
						m_From.playerbet = 100;
					}
					from.SendGump( new PokerGump( from, m_From ) );
					break;
				}
				case 666:
				{ // quit
					m_From.roundend = true;
					m_From.busy = false;
					Effects.PlaySound( from.Location, from.Map, 0x1e9 );
					break;
				}

			}
		}


	public void finishpokergame(Mobile from)
	{
		int i,match1=0,match2=0,match3=0,match4=0,match5=0,temp=0;
		bool isStrt=true,isFlush=false;
		string Temp;

		for ( i = 0; i <= 4; i++ )
			m_From.dealercards[i] = m_From.playercards[i];

		for(int j=4;j>=0;j--)
		{
		for ( i = 0; i < 4; i++ )
		{
		   if (m_From.CardValue2(m_From.dealercards[i])>=m_From.CardValue2(m_From.dealercards[i+1]))
		   {
			temp = m_From.dealercards[i];
			m_From.dealercards[i] = m_From.dealercards[i+1];
			m_From.dealercards[i+1] = temp;
		    }
		}
		}

		for(i=4;i>0;i--)
 		{
 			if(m_From.CardValue2(m_From.dealercards[i])!=(m_From.CardValue2(m_From.dealercards[0])+i))
 			{
				isStrt=false;
				m_From.pokermsg = "Game Over.";
				m_From.pwin = 0;
			}
		}
 
		if((m_From.CardValue2(m_From.dealercards[0])==2) && (m_From.CardValue2(m_From.dealercards[1])==3) && (m_From.CardValue2(m_From.dealercards[2])==4) && (m_From.CardValue2(m_From.dealercards[3])==5) && (m_From.CardValue2(m_From.dealercards[4])==14))
			isStrt=true;
 
		if (isStrt)
		{
		   m_From.pokermsg = "Straight.";
		   m_From.pwin = m_From.playerbet * 4;
		}

		Temp = m_From.CardSuit(m_From.dealercards[0]);
		if(Temp==m_From.CardSuit(m_From.dealercards[1]) && Temp==m_From.CardSuit(m_From.dealercards[2]) && Temp==m_From.CardSuit(m_From.dealercards[3]) && Temp==m_From.CardSuit(m_From.dealercards[4]))
		{
			isFlush=true;
			m_From.pokermsg = "Flush.";
			m_From.pwin = m_From.playerbet * 5;
		}

		if(!isStrt && !isFlush)
		{

		for ( i = 0; i <= 4; i++  )
		{
		   temp = m_From.CardValue2(m_From.dealercards[i]);

		   if ((m_From.CardValue2(m_From.dealercards[0])==temp) && i!=0)
			match1++;
		   if ((m_From.CardValue2(m_From.dealercards[1])==temp) && i!=1)
			match2++;
		   if ((m_From.CardValue2(m_From.dealercards[2])==temp) && i!=2)
			match3++;
		   if ((m_From.CardValue2(m_From.dealercards[3])==temp) && i!=3)
			match4++;
		   if ((m_From.CardValue2(m_From.dealercards[4])==temp) && i!=4)
			match5++;
		}

		if((match1==3)||(match2==3)||(match3==3)||(match4==3)||(match5==3))
		{
			m_From.pokermsg = "4 of a Kind";
			m_From.pwin = m_From.playerbet * 8;
		}

		if((match1==2)||(match2==2)||(match3==2)||(match4==2)||(match5==2))
		{
			m_From.pokermsg = "3 of a Kind";
			m_From.pwin = m_From.playerbet * 3;
		}

		if((match1+match2+match3+match4+match5)==8)
		{
			m_From.pokermsg = "Full House.";
			m_From.pwin = m_From.playerbet * 6;
		}

		if((match1+match2+match3+match4+match5)==4)
		{
			m_From.pokermsg = "Two Pair.";
			m_From.pwin = m_From.playerbet * 2;
		}

		temp = 0;
		if((match1+match2+match3+match4+match5)==2)
		{
		if(match1==1){temp=m_From.CardValue2(m_From.dealercards[0]);} if(match2==1){temp=m_From.CardValue2(m_From.dealercards[1]);}
		if(match3==1){temp=m_From.CardValue2(m_From.dealercards[2]);} if(match4==1){temp=m_From.CardValue2(m_From.dealercards[3]);}
		if(temp>=10)
		{
			m_From.pokermsg = "Pair 10's +";
			m_From.pwin = m_From.playerbet;
		}
		}
		} //end if(isStrt && isFlush)

		if(isFlush && isStrt)
		{
  			if(m_From.dealercards[0]==10)
  			{
  				m_From.pokermsg = "Royal Straight Flush";
  				m_From.pwin = m_From.playerbet * 12;
  			}
  			else
  			{
  				m_From.pokermsg = "Straight Flush";
  				m_From.pwin = m_From.playerbet * 10;
  			}
 		}
 		
 		if (m_From.pwin>0)
 		{
 			m_From.payplayer(from,m_From.pwin);
			Effects.PlaySound( from.Location, from.Map, 0x36 );
			m_From.gamestats[4] += 1;
		}
		else if (m_From.pwin==m_From.playerbet)
		{ m_From.gamestats[5] += 1; }
		else
		{ m_From.gamestats[3] += 1; }

	}
	} //class PokerGump


	public class BlackjackGump : Gump
	{

	private Gambler m_From;

		public BlackjackGump( Mobile mobile, Gambler gambler ) : base( 10, 10 )
		{

			m_From = gambler;

			int i,dealervalue=0,temp=0;
			string cardtemp, scoredmsg, scorepmsg;

			Closable = false;

			AddPage( 0 );

			AddImageTiled( 30, 100, 460, 280, 2624 );
			AddAlphaRegion( 90, 100, 460, 230 );

			AddLabel( 35, 109, 1500, "Dealer:" );
			AddLabel( 35, 229, 600, "Player:" );

			if (m_From.dwin>0)
				AddLabel( 45, 129, 70, ""+m_From.dwin );

			if (m_From.pwin>0)
				AddLabel( 45, 249, 70, ""+m_From.pwin );

			AddButton(  40, 333, 2117, 2118, 101, GumpButtonType.Reply, 0 );
			AddLabel(  60, 330, 800, "Deal" );

			AddButton( 150, 333, 2117, 2118, 102, GumpButtonType.Reply, 0 );
			AddLabel( 170, 330, 800, "Hit" );

			AddButton( 200, 333, 2117, 2118, 103, GumpButtonType.Reply, 0 );
			AddLabel( 220, 330, 800, "Stand" );

			AddButton( 280, 333, 2117, 2118, 104, GumpButtonType.Reply, 0 );
			AddLabel( 300, 330, 800, "Double Down" );

			AddButton(  90, 333, 2117, 2118, 105, GumpButtonType.Reply, 0 );
			AddButton( 33, 363, 3, 4, 666, GumpButtonType.Reply, 0 );

			//show dealer cards
			for ( i = 0; i <= 4; ++i )
			{
				temp = m_From.dealercards[i];
				if (temp>0)
				{
					if (!m_From.dealercardhidden || (m_From.dealercardhidden && i>0))
					{
					    if (NewCards)
						AddImage ( 25 + ((i+1)*75), 110, 4095+temp);
					    else
					    {
						cardtemp = m_From.CardSuit( temp ); //129
						AddBackground( 65 + ((i+1)*40), 110, 35, 50, 2171 );
						AddLabel( 80 + ((i+1)*40), 136, m_From.cardcolor( cardtemp ), cardtemp );
						AddLabel( 72 + ((i+1)*40), 115, 1500, m_From.CardName( temp ) );
					    }

						dealervalue += m_From.CardValue( temp );
					}
					else
					{
					    if (NewCards)
						AddImage ( 25 + ((i+1)*75), 110, 4154);
					    else
					        AddBackground( 65 + ((i+1)*40), 110, 35, 50, 2171 );
					}
				}
			}

			//show player cards
			for ( i = 0; i <= 4; ++i )
			{
				temp = m_From.playercards[i];

				if (temp>0)
				{
				    if (NewCards)
					AddImage ( 25 + ((i+1)*75), 230, 4095+temp);
				    else
				    {
    					cardtemp = m_From.CardSuit( temp );
    					AddBackground( 65 + ((i+1)*40), 230, 35, 50, 2171 );
    					AddLabel( 80 + ((i+1)*40), 256, m_From.cardcolor( cardtemp ), cardtemp );
					AddLabel( 72 + ((i+1)*40), 235, 600, m_From.CardName( temp ) );
				    }
				}
			}

			AddLabel( 110, 330, 800, ""+m_From.playerbet );

			if (!m_From.dealercardhidden)
				dealervalue = dealercardvalue();

			if (m_From.CardValue(m_From.dealercards[0]) + m_From.CardValue(m_From.dealercards[1]) == 21 && !m_From.dealercardhidden)
				scoredmsg = "BJ";
			else
				scoredmsg = dealervalue.ToString();

			if (m_From.CardValue(m_From.playercards[1]) + m_From.CardValue(m_From.playercards[1]) == 21)
				scorepmsg = "BJ";
			else
				scorepmsg = playercardvalue().ToString();
			
			AddLabel( 63, 155, 1500, ""+scoredmsg );
			AddLabel( 63, 274, 600, ""+scorepmsg );
			AddLabel( 100, 350, 64, m_From.pokermsg );

		}


		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			int i=0,temp=0;

			switch ( info.ButtonID )
			{
				case 101:
				{ // deal
					m_From.pokermsg = "ATA- rules";

					if (!from.InRange( m_From.Location, 4 ))
					{
						m_From.roundend = true;
						m_From.busy = false;
					}
					else 
					{
					if (m_From.roundend)
					{
						   if (m_From.playerbet>1000)
						   	m_From.playerbet = 1000;

					if (m_From.paydealer( from, m_From.playerbet))
					{
						m_From.dwin = 0;
						m_From.pwin = 0;
						m_From.roundend = false;
						m_From.dealercardhidden = true;
					// clear dealer and player cards
					for ( i = 2; i <= 4; ++i  )
					{
						m_From.dealercards[i]=0;
						m_From.playercards[i]=0;
					}
						//pick card
						m_From.dealercards[0]=m_From.pickcard(from);
						m_From.playercards[0]=m_From.pickcard(from);
						m_From.dealercards[1]=m_From.pickcard(from);
						m_From.playercards[1]=m_From.pickcard(from);
						
						if (m_From.CardValue(m_From.dealercards[0]) + m_From.CardValue(m_From.dealercards[1]) == 21)
							m_From.dbj = true;
						else if (m_From.CardValue(m_From.playercards[1]) + m_From.CardValue(m_From.playercards[1]) == 21)
							m_From.pbj = true;
						if (m_From.pbj)
							finishgame(from);
					}
					else
						m_From.pokermsg = "You need more money!";

					}
					from.SendGump( new BlackjackGump( from, m_From ) );
					}
					break;
				}
				case 102:
				{ // hit
					if (!m_From.roundend)
					{
						temp=0;
						for ( i = 2; i <= 4; ++i  )
						{
							if (m_From.playercards[i]==0 && temp==0)
							{
								m_From.playercards[i]=m_From.pickcard(from);
								temp = i;
								i=6;
							}
						}

						if ((temp>0 && playercardvalue()<=21) && i!=5)
							from.SendGump( new BlackjackGump( from, m_From ) );
						else
							finishgame( from );

					}
					else
						from.SendGump( new BlackjackGump( from, m_From ) );
					break;
				}
				case 103:
				{ //stand
					if (!m_From.roundend)
						finishgame(from );
					else
						from.SendGump( new BlackjackGump( from, m_From ) );
					break;
				}
				case 104:
				{ //double down
					if (!m_From.roundend)
					{
						temp=0;
						for ( i = 0; i <= 4; ++i  )
						{
							if (m_From.playercards[i]>0)
								temp++;
						}

						if (temp==2 && m_From.paydealer( from, m_From.playerbet))
							   m_From.playerbet *= 2;

						   m_From.playercards[2]=m_From.pickcard(from);
						   finishgame(from );
					}
					else
						from.SendGump( new BlackjackGump( from, m_From ) );
					break;
				}
				case 105:
				{ // bet
					if (m_From.roundend)
					{
					m_From.playerbet += 100;
					if (m_From.playerbet > 1000)
						m_From.playerbet = 100;
					}
					from.SendGump( new BlackjackGump( from, m_From ) );
					break;
				}
				case 666:
				{ // quit
					m_From.roundend = true;
					m_From.busy = false;
					Effects.PlaySound( from.Location, from.Map, 0x1e9 );
					break;
				}

			}
		}

	public void finishgame(Mobile from)
	{
		int i,temp,dealervalue=dealercardvalue(),playervalue=playercardvalue();
		temp = (m_From.playerbet/2);

		if (m_From.dbj && m_From.pbj)
		{
			m_From.dwin = temp;
			m_From.pwin = m_From.playerbet+temp;

			m_From.payplayer(from,m_From.pwin);
			m_From.gamestats[2] += 1;
			m_From.pokermsg = "We have a push.";

		}
		else if (m_From.dbj)
		{
			m_From.gamestats[0] += 1;
			m_From.pokermsg = "Looks like I won.";
			m_From.dwin = m_From.playerbet;
			m_From.pwin = 0;
		}
		else if (m_From.pbj)
		{
			m_From.dwin = 0;
			m_From.pwin = (m_From.playerbet*2)+temp;

			m_From.payplayer(from,m_From.pwin);
			m_From.gamestats[1] += 1;
			m_From.pokermsg = "You won this one.";
		}
		else
		{
		if (playervalue>21 || (dealervalue>playervalue && dealervalue<=21))
		{// dealer won
			m_From.gamestats[0] += 1;
			m_From.pokermsg = "Looks like I won.";
			m_From.dwin = m_From.playerbet;
			m_From.pwin = 0;
		}
		else
		{
		if (dealervalue<17)
		{
			for ( i = 2; i <= 4; ++i  )
			{
				if (m_From.dealercards[i]==0)
				{
					m_From.dealercards[i]=m_From.pickcard(from);
					dealervalue=dealercardvalue();
				}
				if (dealervalue>=17)
					i=6;
			}
		}

		if (playervalue>21 || (dealervalue>playervalue && dealervalue<=21))
		{// dealer won
			m_From.gamestats[0] += 1;
			m_From.pokermsg = "I won this round.";
			m_From.dwin = m_From.playerbet;
			m_From.pwin = 0;
		}
		else if (dealervalue==playervalue)
		{ // tie
			m_From.dwin = temp;
			m_From.pwin = m_From.playerbet+temp;

			m_From.payplayer(from,m_From.pwin);
			m_From.gamestats[2] += 1;
					m_From.pokermsg = "We have a push.";
		}
		else
		{ // count players card

					if (playervalue==21)
		   { // player won
			m_From.dwin = 0;
						m_From.pwin = (m_From.playerbet*2);

			m_From.payplayer(from,m_From.pwin);
			m_From.gamestats[1] += 1;
			m_From.pokermsg = "You have won another round.";
		   }
		   else
		   { // player won
			m_From.dwin = 0;
			m_From.pwin = (m_From.playerbet*2);

			m_From.payplayer(from,m_From.pwin);
			m_From.gamestats[1] += 1;
			m_From.pokermsg = "You won this one.";
		   }
		}
		} // end of else
		}
		m_From.dbj = false;
		m_From.pbj = false;
		m_From.pwin = (m_From.pwin-m_From.playerbet);
		m_From.dealercardhidden = false;
		m_From.roundend = true;
		Effects.PlaySound( from.Location, from.Map, 0x36 );
		from.SendGump( new BlackjackGump( from, m_From ) );
	}

	public int dealercardvalue()
	{
		int i,tempcard=0,gotace=0,dealervalue=0;

		for ( i = 0; i <= 4; ++i  )
		{
			tempcard = m_From.CardValue( m_From.dealercards[i] );
			if (tempcard==11)
				gotace++;

			dealervalue += tempcard;
		}

		while (dealervalue>21 && gotace>0)
		{
			dealervalue -= 10;
			gotace--;
		}

		return dealervalue;
	}

	public int playercardvalue()
	{
		int i,tempcard=0,gotace=0,playervalue=0;

		for ( i = 0; i <= 4; ++i  )
		{
			tempcard = m_From.CardValue( m_From.playercards[i] );
			if (tempcard==11)
				gotace++;

			playervalue += tempcard;
		}

		while (playervalue>21 && gotace>0)
		{
			playervalue -= 10;
			gotace--;
		}

		return playervalue;
	}

}//class BlackjackGump

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );
            writer.Write( (int) 0 ); // version
            writer.Write( (bool) true );
            writer.Write( (bool) false );
		for ( int i = 0; i <= 5; ++i )
			writer.Write( gamestats[i] );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
            roundend = reader.ReadBool();
            busy = reader.ReadBool();
		for ( int i = 0; i <= 5; ++i )
			gamestats[i]=reader.ReadInt();
        }


	public override bool OnGoldGiven( Mobile from, Gold dropped )
	{
	    string message = "Are you trying to bribe me to win?";
	    this.Say( message );
	    return false;
	}

    }
}
