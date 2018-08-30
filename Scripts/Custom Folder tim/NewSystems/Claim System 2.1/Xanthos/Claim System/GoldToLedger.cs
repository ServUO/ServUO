using System;
using Server;
using Server.Items;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Misc;
using Server.Mobiles;
using Server.Gumps;
using Server.Targeting;
using Server.Commands;

namespace Server
{
    public class GoldToLedger
    {
		public static void Deposit( Mobile m, Item item )
		{
			Gold gold = (Gold)item;
			
			 //generate a list of all items in the backpack
            List<Item> packitems = LedgeGoldCommand.RecurseFindItemsInPack(m.Backpack);
			
			item = m.Backpack.FindItemByType(typeof(GoldLedger));
            GoldLedger ledger = item as GoldLedger;

            if (ledger == null)
            {
                m.SendMessage(33, "No gold ledger found.");
                return;
            }
			else
			{
				if( ledger.Gold + gold.Amount < 999999999 )
                {
					gold.Delete();
                    ledger.Gold += gold.Amount;
					m.SendMessage(2125, "{0} gold has been added to your gold ledger", gold.Amount);
				
					if( ledger.b_open )
					{
						m.CloseGump(typeof(GoldLedgerGump));
						m.SendGump(new GoldLedgerGump(ledger));
					}
				}
				
			}
		
		}
	}
}