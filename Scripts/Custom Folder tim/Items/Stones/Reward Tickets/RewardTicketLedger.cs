///daat99's Token System
///Made by daat99 based on idea by Viago :)
///Thanx for Murzin for all the grammer corrections :)
using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	public class RewardTicketLedger : Item
	{
		private int i_Owner, i_RewardTickets;

		[CommandProperty(AccessLevel.Administrator)]
		public int Owner { get { return i_Owner; } set { i_Owner = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.Administrator)]
		public int RewardTickets { get { return i_RewardTickets; } set { i_RewardTickets = value; InvalidateProperties(); } }

		[Constructable]
		public RewardTicketLedger() : base( 7715 )
		{
			Stackable = false;
			Name = "An Empty Reward Ticket Ledger";
			Hue = 1161;
			Weight = 1.0;
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick(Mobile from)
		{
			if ( this.IsChildOf( from.Backpack ) )
			{
				if (i_Owner == 0)
				{
					i_Owner = from.Serial;
					Name = from.Name + "'s Reward Ticket Ledger";
					i_RewardTickets = 0;
					from.SendGump( new RewardTicketsGump( from, this ) );
				}
				else if (from.Serial == i_Owner)
				{
					from.SendGump( new RewardTicketsGump( from, this ) );
				}
				else if (from.AccessLevel >= AccessLevel.GameMaster)
				{
					from.SendMessage(1161, "Select a new owner for this Reward ticket ledger.");
					BeginSetOwner( from );
				}
				else
				{
					from.PlaySound(1074); //play no!! sound
					from.SendMessage(1161, "This book is not yours and you cannot seem to write your name in it!");
				}
			}
			else
				from.SendMessage(1161, "The Reward ticket ledger must be in your backpack to be used.");
		}

		public void BeginSetOwner( Mobile from )
		{
			from.Target = new SetOwnerTarget( this );
		}

		public class SetOwnerTarget : Target
		{
			private RewardTicketLedger m_TL;

			public SetOwnerTarget( RewardTicketLedger tl ) : base( 18, false, TargetFlags.None )
			{
				m_TL = tl;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_TL.Deleted )
					return;

				m_TL.EndSetOwner( from, targeted );
			}
		}

		public void EndSetOwner( Mobile from, object obj )
		{
			if ( obj is PlayerMobile )
			{
				PlayerMobile m = obj as PlayerMobile;
				if ( m.Alive )
				{
					if (!(this.Deleted))
					{
						if  (m.Name != null)
						{
							this.Owner = m.Serial;
							this.Name = m.Name + "'s Reward Ticket Ledger";
							from.SendMessage(1161, "You set the new owner as: {0}", m.Name);
							m.SendMessage(1161, "You became the owner of a new Reward ticket ledger book.");
						}
						else
							from.SendMessage(1161, "Your target doesn't have a name.");
					}
					else
						from.SendMessage(1161, "The ledger was deleted before you selected your target.");
				}
				else
					from.SendMessage(1161, "Your target is dead, please choose a target that is alive.");
			}
			else
				from.SendMessage(1161, "Only players can be the owners of Reward ticket ledgers.");
		}

		public void BeginAddRewardTickets( Mobile from )
		{
			from.Target = new AddRewardTicketsTarget( this );
		}

		public class AddRewardTicketsTarget : Target
		{
			private RewardTicketLedger m_TL;

			public AddRewardTicketsTarget( RewardTicketLedger tl ) : base( 18, false, TargetFlags.None )
			{
				m_TL = tl;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_TL.Deleted )
					return;
				m_TL.EndAddRewardTickets( from, targeted );
			}
		}

		public void EndAddRewardTickets( Mobile from, object obj )
		{
			from.CloseGump( typeof( RewardTicketsGump ) );
			if ( obj is Item )
			{
				Item oldRewardTickets = obj as Item;
				if ( oldRewardTickets.IsChildOf( from.Backpack ) )
				{
					if (oldRewardTickets.Name != null )
					{
						//if ((oldRewardTickets.Name).ToLower().IndexOf("Reward ticket") != -1)
						if ( oldRewardTickets is RewardTicket )
						{
							if (!(this.Deleted) && !(oldRewardTickets.Deleted))
							{
								this.RewardTickets = (this.RewardTickets + oldRewardTickets.Amount);
								from.SendMessage(1161, "You added {0} tickets to your ledger.", oldRewardTickets.Amount);
								oldRewardTickets.Delete();
							}
							else
							{
								from.PlaySound(1069); //play Hey!! sound
								from.SendMessage(1161, "Hey, don't try to rob the bank!!!");
							}
						}
						else
						{
							from.PlaySound(1074); //play no!! sound
							from.SendMessage(1161, "This isn't an Reward ticket!!!");
						}
					}
					else
						from.SendMessage(1161, "The ledger rejected this item.");
				}
				else
					from.SendMessage(1161, "This isn't in your backpack.");
			}
			else
				from.SendMessage(1161, "This is not an item.");
			from.SendGump( new RewardTicketsGump( from, this ) );
		}

		public RewardTicketLedger( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( i_RewardTickets );
			writer.Write( i_Owner );

		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch (version)
			{
				case 0:
				{
					i_RewardTickets = reader.ReadInt();
					i_Owner = reader.ReadInt();
					break;
				}
			}
		}
	}

	public class RewardTicketsGump : Gump
	{
		private Mobile m_From;
		private RewardTicketLedger m_TL;

		public RewardTicketsGump( Mobile from, Item item ) : base( 50, 50 )
		{
			from.CloseGump( typeof( RewardTicketsGump ) );
			m_From = from;
			if (!(item is RewardTicketLedger))
				return;
			RewardTicketLedger tl = item as RewardTicketLedger;
			m_TL = tl;

			AddPage(0);

			AddBackground(40, 40, 360, 325, 5170);

			AddLabel(75, 75, 69, item.Name);
			AddLabel(75, 100, 88, "You have " + ((RewardTicketLedger)tl).RewardTickets + " Reward tickets.");
			AddLabel(75, 125, 32, @"Add tickets to your Ledger manually:");
			AddButton(307, 130, 2460, 2461, 1, GumpButtonType.Reply, 0); //add tickets
			AddLabel(75, 150, 88, @"How many ticket do you want to take out?");

			AddBackground(125, 200, 195, 41, 9270); //text entry backgrounf
			if (((RewardTicketLedger)tl).RewardTickets >= 10000)
				AddTextEntry(145, 211, 155, 21, 39, 3, "10000"); //default text entry (where we write how much RewardTickets)
			else
				AddTextEntry(145, 211, 155, 21, 39, 3, "" + ((RewardTicketLedger)tl).RewardTickets + "");

			//ticket item
			AddLabel(75, 180, 69, @"Extract tickets:");
			AddImage(70, 200, 92);
			AddButton(79, 207, 2474, 0, 2, GumpButtonType.Reply, 0);

			AddImage(70, 255, 7012);
			AddImage(300, 255, 7012);
			AddLabel(146, 277, 38, @"Reward Ticket System");
		}
		public override void OnResponse( NetState state, RelayInfo info )
		{
			if ( m_TL.Deleted )
				return;
			else if ( info.ButtonID == 1 )
			{
				if (((RewardTicketLedger)m_TL).RewardTickets <= 1000000)
					m_TL.BeginAddRewardTickets( m_From );
				else
					m_From.SendMessage(1161, "This Reward ticket ledger is full, pick up another ledger in the lounge");
				m_From.SendGump( new RewardTicketsGump( m_From, m_TL ) );
			}
			else if ( info.ButtonID == 2 )
			{
				TextRelay tr_RewardTicketAmount = info.GetTextEntry( 3 );
				if(tr_RewardTicketAmount != null)
				{
					int i_MaxAmount = 0;
					try
					{
						i_MaxAmount = Convert.ToInt32(tr_RewardTicketAmount.Text,10);
					}
					catch
					{
						m_From.SendMessage(1161, "Please make sure you write only numbers.");
					}
					if(i_MaxAmount > 0)
					{
						if (i_MaxAmount <= ((RewardTicketLedger)m_TL).RewardTickets)
						{
							if (i_MaxAmount <= 100)
							{
								for ( int i = 0; i < i_MaxAmount; ++i )
									m_From.AddToBackpack(new RewardTicket());
								m_From.SendMessage(1161, "You extracted {0} tickets from your ledger.", i_MaxAmount);
									((RewardTicketLedger)m_TL).RewardTickets = (((RewardTicketLedger)m_TL).RewardTickets - i_MaxAmount);
							}
							else
								m_From.SendMessage(1161, "You can't extract more than 100 tickets at a time.");
						}
						else
							m_From.SendMessage(1161, "You don't have that many tickets in your ledger.");
					}
					m_From.SendGump( new RewardTicketsGump( m_From, m_TL ) );
				}
			}
		}
	}
}
