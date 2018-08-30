using System;
using Server;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Accounting;
using Server.Mobiles;



namespace Server.Gumps
{
	
	public class SoundPickerGump : Server.Gumps.Gump
	{
		
		public static void Initialize()
		{
			CommandSystem.Register( "SoundPicker", AccessLevel.GameMaster, new CommandEventHandler( Sounds_OnCommand ) );
			
		}

		[Usage( "SoundPicker" )]
		[Description( "Allows you to scroll through Sound Ids" )]
		private static void Sounds_OnCommand( CommandEventArgs e )
		{

			Mobile from = e.Mobile;
			from.CloseGump(typeof(SoundPickerGump));
            from.SendGump(new SoundPickerGump(from, 0));
           
		}
		
		public TextRelay SoundID;
		public int SoundVal;

				
		public SoundPickerGump(Mobile from, int SoundVal) : base(0, 0)
		{
			Closable=true;
			Disposable=true;
			Dragable=true;
			Resizable=false;
			AddPage(0);
			
			AddBackground(31, 65, 300, 180, 9270);
			AddLabel(85, 75, 2025, @"Sound Picker:");

			AddLabel(95, 110, 1153, @"Sound:");
			AddBackground(140, 110, 40, 20, 9300);
			AddTextEntry(140, 110, 40, 20, 1153, (int)Buttons.SoundTxt, "" + SoundVal + "");
			
			AddLabel(130, 150, 1153, @"Previous");
			AddButton(140, 170, 5537, 5539, 1, GumpButtonType.Reply, 0); //Previous Body Type
			
			AddLabel(190, 150, 1153, @"Next");
			AddButton(200, 170, 5540, 5542, 2, GumpButtonType.Reply, 0); //Next Body Type
			
			AddButton(190, 110, 4023, 4024, 3, GumpButtonType.Reply, 0); // Okay to input Type
			
			AddButton(220, 110, 4020, 4021, 4, GumpButtonType.Reply, 0); // Cancel/Close
		}
		
			
		public enum Buttons
		{
			SoundTxt = 1,
			OkBtn,
			CancelBtn
		}
		
		public override void OnResponse( NetState sender, RelayInfo info )
		{
		
			 Mobile from = sender.Mobile;
			
			switch( info.ButtonID )
			{
				case 1:
				{
					SoundID = info.GetTextEntry(1);
									
					try
					{
						SoundVal = Convert.ToInt32(SoundID.Text);
					}
					catch
					{
						from.SendMessage( "Numbers only, please try again." );
						break;
					}
					
						SoundVal -= 1;
						Effects.PlaySound(from, from.Map, SoundVal);
						from.SendGump(new SoundPickerGump(from, SoundVal));
						break;
				}
				
				case 2:
				{
					SoundID = info.GetTextEntry(1);
									
					try
					{
						SoundVal = Convert.ToInt32(SoundID.Text);
					}
					catch
					{
						from.SendMessage( "Numbers only, please try again." );
						break;
					}
					
						SoundVal += 1;
						Effects.PlaySound(from, from.Map, SoundVal);
						from.SendGump(new SoundPickerGump(from, SoundVal));
						break;
				}
				case 3:
				{
					SoundID = info.GetTextEntry(1);
									
					try
					{
						SoundVal = Convert.ToInt32(SoundID.Text);
					}
					catch
					{
						from.SendMessage( "Numbers only, please try again." );
						break;
					}
					
						Effects.PlaySound(from, from.Map, SoundVal);
						from.SendGump(new SoundPickerGump(from, SoundVal));
						break;
				}
				case 4:
				{
					from.CloseGump( typeof( SoundPickerGump) );
					break;
				}
			}			
		}
	}
}