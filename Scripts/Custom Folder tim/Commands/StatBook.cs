//REmade by GOOBER Programming!

using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Network;

namespace Server.Items 
{
	public class StatBook : Item
	{ 
		[Constructable] 
		public StatBook() :  base( 0x577F ) 
		{ 
			Weight = 1.0;
			Name = "-Stat book-"; 
			LootType = LootType.Blessed;
			Movable =  false;
		}

		public override void OnDoubleClick( Mobile m ) 
		{				
			m.CloseGump( typeof( StatBookGump ) );
			m.SendGump ( new StatBookGump() );
		} 

		public StatBook( Serial serial ) : base( serial ) 
		{ 
		} 
       
		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 1 ); // version 
		}

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt(); 
		} 
	}
}

namespace Server.Gumps
{
	public class StatBookGump : Server.Gumps.Gump
	{
		
		public TextRelay StrEnt;
		public TextRelay DexEnt;
		public TextRelay IntEnt;
		
		public StatBookGump() : base(0, 0)
		{
			this.Closable=true;
			this.Disposable=false;
			this.Dragable=true;
			this.Resizable=false;
			this.AddPage(0);
			this.AddBackground(31, 65, 115, 142, 9270);
			this.AddLabel(45, 80, 1153, @"Str:");
			this.AddLabel(45, 110, 1153, @"Dex:");
			this.AddLabel(45, 140, 1153, @"Int:");
			this.AddBackground(90, 110, 40, 20, 9300);
			this.AddBackground(90, 140, 40, 20, 9300);
			this.AddBackground(90, 80, 40, 20, 9300);
			this.AddTextEntry(90, 80, 40, 20, 1153, (int)Buttons.StrTxt, @"");
			this.AddTextEntry(90, 110, 40, 20, 1153, (int)Buttons.DexTxt, @"");
			this.AddTextEntry(90, 140, 40, 20, 1153, (int)Buttons.IntTxt, @"");
			this.AddButton(100, 170, 4023, 4024, (int)Buttons.OkBtn, GumpButtonType.Reply, 0);
			this.AddButton(65, 170, 4020, 4021, (int)Buttons.CancelBtn, GumpButtonType.Reply, 0);
		}
		
		public enum Buttons
		{
			StrTxt = 1,
			DexTxt = 2,
			IntTxt = 3,
			OkBtn,
			CancelBtn
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile m = state.Mobile;
			
			Item i = m.Backpack.FindItemByType( typeof( StatBook ) );
			
			switch( info.ButtonID )
			{
				case (int)Buttons.OkBtn:
				{
					StrEnt = info.GetTextEntry(1);
					DexEnt = info.GetTextEntry(2);
					IntEnt = info.GetTextEntry(3);
										
					int StrVal;
					int DexVal;
					int IntVal;
					
					try
					{
						StrVal = Convert.ToInt32(StrEnt.Text);
						DexVal = Convert.ToInt32(DexEnt.Text);
						IntVal = Convert.ToInt32(IntEnt.Text);
					}
					catch
					{
						m.SendMessage( "Numbers only, please try again." );
						break;
					}
					
					if( StrVal > 200 || StrVal < 10 || DexVal > 200 || DexVal < 10 || IntVal > 200 || IntVal < 10 )
					{
						m.SendMessage( "Stats can be between 10-150, please try again." );
						break;
					}
					else if( StrVal + DexVal + IntVal != m.StatCap )
					{
						m.SendMessage( "The stats you've entered equal {0}, your stats cannot exceed, nor be below {1}, Dont be greedy! But dont Under do it!" , StrVal + DexVal + IntVal, m.StatCap );
						break;
					}
					else
					{
						m.RawStr = StrVal;
						m.RawDex = DexVal;
						m.RawInt = IntVal;
						m.Hits	 = m.HitsMax;
						m.Stam	 = m.StamMax;
						m.Mana	 = m.ManaMax;
						
						m.SendMessage( "Your stats have been changed." );
						
						i.Delete();
						
						break;
					}					
										
					break;
				}
				
				case (int)Buttons.CancelBtn:
				{
					m.CloseGump( typeof( StatBookGump) );
					break;
				}
			}			
		}
	}
}
