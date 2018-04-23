using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Items;
using Server.Engines.CannedEvil;


namespace Server.Gumps
{
    public class PSCreditsGump : Gump
    {

        public PSCreditsGump() : base( 0, 0 )
        {
            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;

			AddPage(0);
			AddBackground(1, 0, 789, 382, 9200);
			AddLabel(251, 18, 39, @"Powerscroll Exchange System");
			AddItem(522, 11, 5360, 1153);
			AddHtml( 449, 94, 285, 256, @"Welcome to the Powerscroll Exchange System.  You may close this gump, and drop powerscrolls on the barrel to obtain Powerscroll Credits.  These credits may be used on this menu to buy better Powerscrolls.  The scroll you buy will be a random skill.", (bool)true, (bool)true);
			AddButton(24, 122, 247, 248, 1, GumpButtonType.Reply, 0);
			AddButton(23, 158, 247, 248, 2, GumpButtonType.Reply, 0);
			AddButton(24, 193, 247, 248, 3, GumpButtonType.Reply, 0);
			AddLabel(84, 76, 1171, @"Purchase Scrolls");
			AddLabel(104, 123, 0, @"110 Powerscroll (10 Credits)");
			AddLabel(105, 157, 0, @"115 Powerscroll (100 Credits)");
			AddLabel(104, 193, 0, @"120 Powerscroll (1000 Credits)");
			AddLabel(23, 348, 0, @"By:  DxMonkey");
			

            
        }

        

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;

            switch(info.ButtonID)
            {
                case 0:
				{
					 from.SendMessage( 68, "You choose not to purchase any Powerscrolls");
                        return;

				}
				
				case 1:
				{
					Item[] Token = from.Backpack.FindItemsByType( typeof( PSCredits ) );
					if ( from.Backpack.ConsumeTotal( typeof( PSCredits ), 10 ) )
						{
							PowerScroll ps = CreateRandomPowerScroll();
							ps.Value = 110;
							from.AddToBackpack( ps );
							from.SendMessage( "You buy a random 110 Powerscroll." );
						}
					else
						{
							from.SendMessage( "You do not have enough funds for that." );
						}
					break;
				}
				
				case 2:
				{
					Item[] Token = from.Backpack.FindItemsByType( typeof( PSCredits ) );
					if ( from.Backpack.ConsumeTotal( typeof( PSCredits ), 100 ) )
						{
							PowerScroll ps = CreateRandomPowerScroll();
							ps.Value = 115;
							from.AddToBackpack( ps );
							from.SendMessage( "You buy a random 115 Powerscroll." );
						}
					else
						{
							from.SendMessage( "You do not have enough funds for that." );
						}
				
					break;
				}
				
				case 3:
				{
					Item[] Token = from.Backpack.FindItemsByType( typeof( PSCredits ) );
					if ( from.Backpack.ConsumeTotal( typeof( PSCredits ), 1000 ) )
						{
							PowerScroll ps = CreateRandomPowerScroll();
							ps.Value = 120;
							from.AddToBackpack( ps );
							from.SendMessage( "You buy a random 120 Powerscroll." );
						}
					else
						{
							from.SendMessage( "You do not have enough funds for that." );
						}
				
					break;
				}
            }
        }
		public PowerScroll CreateRandomPowerScroll()
		{
			int level;
			double random = Utility.RandomDouble();

			if ( 0.05 >= random )
				level = 20;
			else if ( 0.4 >= random )
				level = 15;
			else
				level = 10;

			return PowerScroll.CreateRandomNoCraft( level, level );
		}
    }
}