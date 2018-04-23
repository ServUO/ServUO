using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Network;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "Charon corpse" )]
	public class Charon : Mobile
	{
                public virtual bool IsInvulnerable{ get{ return true; } }
		[Constructable]
		public Charon()
		{
			Name = "Charon";
                        Title = "The Ferryman of the River Styx";
			Body = 400;
			CantWalk = true;
                        AddItem( new Sandals() );
			AddItem( new Robe()); 
			AddItem( new GnarledStaff() );
			int hairHue = 1153;
			

			switch ( Utility.Random( 1 ) )
			{
				case 0: AddItem( new LongHair( hairHue ) ); break;
			} 
			
			Blessed = true;
			
			}

			public virtual int GetBootsHue()
			{
			return 1157;
		}

		public Charon( Serial serial ) : base( serial )
		{
		}

		

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}

		public class CharonEntry : ContextMenuEntry
		{
			private Mobile m_Mobile;
			private Mobile m_Giver;
			
			public CharonEntry( Mobile from, Mobile giver ) : base( 6146, 3 )
			{
				m_Mobile = from;
				m_Giver = giver;
			}

			public override void OnClick()
			{
				

                          if( !( m_Mobile is PlayerMobile ) )
					return;
				
				PlayerMobile mobile = (PlayerMobile) m_Mobile;

				{
					if ( ! mobile.HasGump( typeof( CharonGump ) ) )
					{
						mobile.SendGump( new CharonGump( mobile ));
						
					} 
				}
			}
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{          		
         	        Mobile m = from;
			PlayerMobile mobile = m as PlayerMobile;

			if ( mobile != null)
			{
				if( dropped is Obolus )
         		{
         			if(dropped.Amount!=1)
         			{
					this.PrivateOverheadMessage( MessageType.Regular, 1153, false, "Good Luck Hope I Dont See You Too Soon", mobile.NetState );
         				return false;
         			}
 
					mobile.SendMessage( "Good Luck Hope I Dont See You Too Soon" );

				int rand = Utility.Random(1);
				switch (rand)
				{				
					case 0:
						m.SendMessage("Good Luck Hope I Dont See You Too Soon");
						m.Map = Map.Ilshenar;
						m.Location = new Point3D(338,674,-28);
						break;
				}
					return true;
         		}
				else if ( dropped is Obolus )
				{
				this.PrivateOverheadMessage( MessageType.Regular, 1153, 1054071, mobile.NetState );
         			return false;
				}
         		else
         		{
					this.PrivateOverheadMessage( MessageType.Regular, 1153, false, "Why on earth would I want to have that?", mobile.NetState );
     			}
			}
			return false;
		}
	}
}
