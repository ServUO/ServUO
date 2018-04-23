using System;
using Server;
using Server.Gumps;
using Server.Network;
using Server.Commands;
using Server.Accounting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Server.Gumps
{
    public class AccountGoldSearch : Gump
    {
        Mobile caller;
		public static List<Account> acct;

        public static void Initialize()
        {
			CommandSystem.Register("AccountGoldSearch", AccessLevel.Administrator, new CommandEventHandler( AccountGoldSearch_OnCommand));
		}

        [Usage("AccountGoldSearch")]
		[Description("")]
        public static void AccountGoldSearch_OnCommand(CommandEventArgs e)
        {
            Mobile caller = e.Mobile;
			
			if( !AccountGold.Enabled  )
			{
				caller.SendMessage("Account Gold is not currently enabled");
				return;
			}
            if (caller.HasGump(typeof(AccountGoldSearch)))
                caller.CloseGump(typeof(AccountGoldSearch));
				
			acct = new List<Account>();
			
			foreach (Account a in Accounts.GetAccounts())
            {
              acct.Add(a);
			}
			
            caller.SendGump(new AccountGoldSearch(caller, 0, acct));
        }

        
		public int Page {get; set;}
		
		
		
        public AccountGoldSearch(Mobile from, int page,  List<Account> _acct) : base( 0, 0 )
        {
            this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			caller = from;
			Page = page;
			AddPage(0);
			
			AddBackground(118, 0, 522, 614, 9250);
			AddBackground(135, 51, 488, 548, 9350);
			AddLabel(135, 15, 87, @"Account Gold Search");
			
			AddLabel(144, 55, 0, @"Acct Name");
			AddButton(218, 56, 5208, 5208, 3, GumpButtonType.Reply, 0); 
			AddButton(241, 56, 5209, 5209, 4, GumpButtonType.Reply, 0); 
			
			AddLabel(295, 54, 0, @"Gold Balance");
			AddButton(380, 56, 5208, 5208, 5, GumpButtonType.Reply, 0); 
			AddButton(403, 56, 5209, 5209, 6, GumpButtonType.Reply, 0); 
			
			AddLabel(455, 55, 0, @"Plat Balance");
			AddButton(539, 56, 5208, 5208, 7, GumpButtonType.Reply, 0); 
			AddButton(562, 56, 5209, 5209, 8, GumpButtonType.Reply, 0); 
			
			AddImageTiled(143, 77, 472, 1, 10000);
			
			int AmountPerPage = 20;
			
			if( Page > 0 )
				AddButton(455, 30, 2468, 2468, 1, GumpButtonType.Reply, 0); // previous
			if( (Page + 1) * AmountPerPage < acct.Count ) 
				AddButton(550, 30, 2471, 2471, 2, GumpButtonType.Reply, 0); // next
			
			for (int i = 0; i < AmountPerPage && (Page * AmountPerPage + i) < acct.Count; ++i)
            {
				AddLabel(145, 85 + i * 25, 0, String.Format("{0}", acct[(Page * AmountPerPage + i)].Username) );
				AddLabel(295, 85 + i * 25, 0, String.Format("{0}", acct[(Page * AmountPerPage + i)].TotalGold.ToString("#,0")) );
				AddLabel(455, 85 + i * 25, 0, String.Format("{0}", acct[(Page * AmountPerPage + i)].TotalPlat.ToString("#,0")) );
				AddImageTiled(143, 105 + i * 25, 472, 1, 10000);
			}
		}

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            Mobile from = sender.Mobile;
			
			List<Account> newlist = new List<Account>();
			foreach (Account a in Accounts.GetAccounts())
			{
				newlist.Add(a);
			}

            switch(info.ButtonID)
            {
				case 0: 
				{
					if( from.HasGump(typeof(AccountGoldSearch)) ) 
					{
						from.CloseGump(typeof(AccountGoldSearch));
					}
					break;
				}
				case 1:
				{
					Page--;
					from.SendGump(new AccountGoldSearch(from, Page, acct));
					break;
				}
				case 2:
				{
					Page++;
					from.SendGump(new AccountGoldSearch(from, Page, acct));
					break;
				}
				case 3:
				{
					acct = newlist.OrderBy(o => o.Username).ToList();
					from.SendGump(new AccountGoldSearch(from, Page, acct));
					break;
				}
				case 4:
				{
					acct = newlist.OrderByDescending(o => o.Username).ToList();
					from.SendGump(new AccountGoldSearch(from, Page, acct));
					break;
				}
				case 5:
				{
					acct = newlist.OrderBy(o => o.TotalGold).ToList();
					from.SendGump(new AccountGoldSearch(from, Page, acct));
					break;
				}
				case 6:
				{
					acct = newlist.OrderByDescending(o => o.TotalGold).ToList();
					from.SendGump(new AccountGoldSearch(from, Page, acct));
					break;
				}
				case 7:
				{
					acct = newlist.OrderBy(o => o.TotalPlat).ToList();
					from.SendGump(new AccountGoldSearch(from, Page, acct));
					break;
				}
				case 8:
				{
					acct = newlist.OrderByDescending(o => o.TotalPlat).ToList();
					from.SendGump(new AccountGoldSearch(from, Page, acct));
					break;
				}

            }
			
        }
    }
}