/*
 * MODIFICATIONS
 *
 * NUMÉRO        DATE            AUTEUR       
 * ------        ----------      ------        
 * #01         2007-11-19      gargouille
 *    > Peut revendiquer les corps tués par ses créatures, summoned ou tamed
 * * */

using System;
using Server;
using Server.Items;
using Server.Network;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Gumps
{
	public class MonsterContractGump : Gump
	{
		private MonsterContract MCparent;
		
		public MonsterContractGump( Mobile from, MonsterContract parentMC ) : base( 0, 0 )
		{
			if(from != null)from.CloseGump( typeof( MonsterContractGump ) );
			
			if(parentMC != null)
			{
				
				MCparent = parentMC;
			
				this.Closable=true;
				this.Disposable=true;
				this.Dragable=true;
				this.Resizable=false;

				this.AddPage(0);
				this.AddBackground(0, 0, 300, 170, 5170);
				this.AddLabel(40, 40, 0, @"Contract For: " + parentMC.AmountToKill + " " + MonsterContractType.Get[parentMC.Monster].Name);
				this.AddLabel(40, 60, 0, @"Quantity Killed: " + parentMC.AmountKilled);
				this.AddLabel(40, 80, 0, @"Reward: " + parentMC.Reward);
				if ( parentMC.AmountKilled != parentMC.AmountToKill )
				{
					this.AddButton(90, 110, 2061, 2062, 1, GumpButtonType.Reply, 0);
					this.AddLabel(104, 108, 0, @"Corpse");
				}
				else
				{
					this.AddButton(90, 110, 2061, 2062, 2, GumpButtonType.Reply, 0);
					this.AddLabel(104, 108, 0, @"Reward");
				}
			}
		}
		
		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile m_from = state.Mobile;
			
			if(m_from != null && MCparent != null)
			{
				if ( info.ButtonID == 1 )
				{
					m_from.SendMessage("Choose the body to be added.");
					m_from.Target = new MonsterCorpseTarget( MCparent );
				}
				if ( info.ButtonID == 2 )
				{
					m_from.SendMessage("Your reward was placed in your bank!");
					m_from.BankBox.DropItem( new BankCheck( MCparent.Reward ) );
					MCparent.Delete();
				}
			}
		}
	}
	
	public class MonsterCorpseTarget : Target
	{
		private MonsterContract MCparent;
		
		public MonsterCorpseTarget( MonsterContract parentMC ) : base( -1, true, TargetFlags.None )
		{
			MCparent = parentMC;
		}
		
		protected override void OnTarget( Mobile from, object o )
		{
            if ( MCparent == null || from == null || o == null || MCparent.Monster == null)
            {
                Console.WriteLine( "MonsterContract: Sa bug !! Mais où, on sait pas :p" );
                return;
            }
			if ( o is Corpse )
			{
				Corpse MCcorpse = (Corpse)o;
				
				if ( MCcorpse.Channeled )
				{
					from.SendMessage("This body was profaned and cannot be used.");
					return;
				}
				if ( MCcorpse.Killer == from || IsValidFor(from, MCcorpse))
				{
					if ( MCcorpse.Owner.GetType() == MonsterContractType.Get[MCparent.Monster].Type )
					{
						MCparent.AmountKilled += 1;
						MCcorpse.Delete();	
					}
					else
						from.SendMessage("This corpse is no good.");
				}
				else
					from.SendMessage("You cannot use a body killed by another.");
			}
			else
			{
				try{from.SendMessage("This is not a corpse.");}
				catch{Console.WriteLine("Crash sur monstercontractgump from == null");}
			}
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
