using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Gumps
{
	public class MonsterContractBookGump : Gump
	{
		private Mobile m;
		private MonsterContractBook b;
		
		public MonsterContractBookGump( Mobile from, MonsterContractBook book ) : base( 0, 0 )
		{
			from.CloseGump( typeof( MonsterContractBookGump ) );
			
			this.Closable=true;
			this.Disposable=true;
			this.Dragable=true;
			this.Resizable=false;
			
			m = from;
			b = book;
			
			this.AddPage(0);
						
			this.AddBackground(8, 10, 457, 80+(book.Entries.Count*17), 9200);
			
			this.AddAlphaRegion(142, 21, 201, 20);
			this.AddLabel(158, 22, 0, @"MONSTER CONTRACT BOOK");
			
			this.AddAlphaRegion(28, 52, 150, 15);
			this.AddAlphaRegion(180, 52, 70, 15);
			this.AddAlphaRegion(252, 52, 70, 15);
			this.AddAlphaRegion(324, 52, 100, 15);
			this.AddLabel(30, 51, 0, @"Name");
			this.AddLabel(182, 51, 0, @"Killed");
			this.AddLabel(254, 51, 0, @"To Kill");
			this.AddLabel(326, 51, 0, @"Reward");
			
			for( int i = 0; i < book.Entries.Count; ++i)
			{
				MonsterContractEntry MCE = book.Entries[i] as MonsterContractEntry;
				this.AddAlphaRegion(28, 71+(i*17), 150, 15);
				this.AddAlphaRegion(180, 71+(i*17), 70, 15);
				this.AddAlphaRegion(252, 71+(i*17), 70, 15);
				this.AddAlphaRegion(324, 71+(i*17), 100, 15);
				this.AddLabel(29, 70+(i*17), 0, ""+MonsterContractType.Get[MCE.Monster].Name);
				this.AddLabel(182, 70+(i*17), 0, ""+MCE.AmountKilled);
				this.AddLabel(254, 70+(i*17), 0, ""+MCE.AmountToKill);
				this.AddLabel(326, 70+(i*17), 0, ""+MCE.Reward);
				this.AddButton(429, 73+(i*17), 2362, 2362, 200+i, GumpButtonType.Reply, 0);
				this.AddButton(444, 73+(i*17), 2360, 2360, 100+i, GumpButtonType.Reply, 0);
			}
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( info.ButtonID > 0 )
			{
				if( info.ButtonID >= 200 )// Add Corpse
				{
					m.SendMessage("Choose the corpse to add.");
					m.Target = new MonsterCorpseBookTarget( b,info.ButtonID % 100 );
				}
				else if ( info.ButtonID >= 100 ) // One removes the deed book.
				{
					MonsterContractEntry MCE = b.Entries[ info.ButtonID % 100 ] as MonsterContractEntry;
					MonsterContract MC = new MonsterContract( MCE.Monster, MCE.AmountKilled, MCE.AmountToKill, MCE.Reward );
					m.AddToBackpack( MC );
					b.Entries.RemoveAt( info.ButtonID % 100 );
				}
				
				m.SendGump( new MonsterContractBookGump( (PlayerMobile) m, b ) );//#01
			}
		}
	}
	public class MonsterCorpseBookTarget : Target
	{
		private MonsterContractEntry MCE;
		private MonsterContractBook b;
		
		public MonsterCorpseBookTarget( MonsterContractBook book, int i ) : base( -1, true, TargetFlags.None )
		{
			MCE =  book.Entries[i] as MonsterContractEntry;
			b = book;
		}
		
		protected override void OnTarget( Mobile from, object o )
		{
			if ( o is Corpse )
			{
				Corpse MCcorpse = (Corpse)o;
				
				if ( MCcorpse.Channeled )
				{
					from.SendMessage("This corpse has been profaned and cannot be added.");
					return;
				}
				if ( MCcorpse.Killer == from || IsValidFor(from, MCcorpse))
				{
					if ( MCcorpse.Owner.GetType() == MonsterContractType.Get[MCE.Monster].Type )
					{
						MCE.AmountKilled += 1;
						MCcorpse.Delete();
						from.SendGump( new MonsterContractBookGump( (PlayerMobile) from, b ) );
					}
					else
						from.SendMessage("This body isn't acceptable.");
				}
				else
					from.SendMessage("You cannot claim corpses killed by another.");
			}
			else
				from.SendMessage("This is not a corpse.");
		}
	
		private bool IsValidFor(Mobile from, Corpse corpse)
		{
			if(corpse != null)
			{
				Mobile killer = corpse.Killer;
				
				if(killer!=null)
				{
					if(killer is BaseCreature)
					{
						BaseCreature bc = (BaseCreature)killer;
						if((bc.Summoned && bc.SummonMaster==from)||(bc.Controlled && bc.ControlMaster==from))
							return true;
					}
				}
			}
			return false;
		}
		
	}
}
