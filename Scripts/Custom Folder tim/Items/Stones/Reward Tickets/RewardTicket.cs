/************************************************************************
*
* This script was made by milt, AKA Pokey.
*
* Email: pylon2007@gmail.com
*
* AIM: TrueBornStunna
*
* Website: www.f13nd.net
*
* Version: 1.0.3
*
* Release Date: December 25, 2005
*
*************************************************************************/
using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;

namespace Server.Items
{
	public class RewardTicket : Item
	{
		[Constructable]
		public RewardTicket() : base( 0x14F0 )
		{
			Name = "a Reward ticket";
			Movable = true;
			LootType = LootType.Blessed;
			Hue = 1161;
		}

		public RewardTicket( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick(Mobile from)
		{
			if ( this.IsChildOf( from.Backpack ) )
			{
				// keithn - Add to Reward Ticket Ledger
				Item[] items = from.Backpack.FindItemsByType( typeof( RewardTicketLedger ) );

				foreach( RewardTicketLedger ttl in items )
				{
					if ( ttl.Owner == from.Serial )
					{
						if ((ttl.RewardTickets + 1) <= 1000000 )
						{
							if (!(this.Deleted))
							{
								from.CloseGump( typeof( RewardTicketsGump ) );
								ttl.RewardTickets = (ttl.RewardTickets + 1);
								from.SendMessage( 1161, "Reward ticket has been added to your Reward ticket ledger.");
								from.SendMessage( 1161, "Redeem Reward tickets at the Reward vendor stone to receive your prize or save them up to get better prizes!");
								from.SendGump( new RewardTicketsGump( from, ttl ) );
								this.Delete();
								break;
							}
							else
							{
								from.PlaySound(1069); //play Hey!! sound
								from.SendMessage(1173, "Hey, don't try to rob the bank!!!");
								from.SendGump( new RewardTicketsGump( from, this ) );
							}
						}
						else
						{
							from.SendMessage(1173, "You have a full Reward ticket ledger, please pick up an extra ledger in the lounge.");
						}
					}
				}
			}
			else
			{
				from.SendMessage(1173, "The Reward ticket must be in your backpack to be used.");
			}
			//keithn - Add to Reward Ticket Ledger - End
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
