using System;
using Server;
using Server.Factions;
using System.Collections;
using System.Collections.Generic;
using Server.Gumps;
using Server.Guilds;
using Server.Items;
using Server.Engines.LotterySystem;

namespace Server.Mobiles
{
	public class LottoTicketVendor : Mobile
	{
        
        [Constructable]
        public LottoTicketVendor()
		{
            Name = NameList.RandomName("female");
            Title = "the lottery attendant";

            Female = true;
            NameHue = 0x35;
            BodyValue = 401;
            Frozen = true;
            Blessed = true;

            Hue = Utility.RandomSkinHue();
            AddItem(new Sandals());
            AddItem(new Kilt(Utility.RandomBlueHue()));
            AddItem(new FeatheredHat(Utility.RandomGreenHue()));
            AddItem(new FancyShirt(Utility.RandomBlueHue()));

            Hits = HitsMax;
            Stam = StamMax;
            Mana = ManaMax;
		}

        public override bool OnDragDrop(Mobile from, Item dropped)
        {
            if (dropped is BaseLottoTicket)
            {
                BaseLottoTicket ticket = (BaseLottoTicket)dropped;
                int payOut = ticket.Payout;
                Container pack = from.Backpack;

                if (!ticket.Checked)
                {
                    Say("You haven't played this ticket yet!");
                    return false;
                }

                if (!ticket.CashedOut && ticket.FreeTicket)
                    GiveFreeTicket(from, ticket);

                if (ticket.CashedOut)
                    Say("This ticket has already been cashed out!");

                else if (payOut == 0)
                    Say("I'm sorry, but this ticket is not a winning ticket.");

                else
                {
                    if (payOut <= 1000000 && pack != null)
                    {
                        pack.DropItem(new BankCheck(payOut));
                        from.SendMessage("Your winnings of {0} has been placed into your backpack. Please play again.", payOut);
                    }
                    else
                    {
                        Banker.Deposit(from, payOut);
                        from.SendMessage("Your winnings of {0} has been deposited into your bankbox. Please play again.", payOut);
                    }

                    from.PlaySound(52);
                    from.PlaySound(53);
                    from.PlaySound(54);
                    from.PlaySound(55);
                    ticket.CashedOut = true;
                }

                dropped.Delete();
            }

            return false;
        }

        private void GiveFreeTicket(Mobile from, BaseLottoTicket ticket)
        {
            if (from == null) return;

            Item item = null;
            string name = "";

            switch (ticket.Type)
            {
                default:
                case TicketType.GoldenTicket: item = new GoldenTicket(from, false); name = "Golden Ticket"; break;
                case TicketType.CrazedCrafting: item = new CrazedCrafting(from, false); name = "Crazed Crafting"; break;
                case TicketType.SkiesTheLimit: item = new SkiesTheLimit(from, false); name = "Skies the Limit"; break;
            }

            if (item != null)
            {
                from.SendMessage("You have recived your free {0} ticket.", name);
                if (from.Backpack != null)
                    from.Backpack.DropItem(item);
                else
                    from.BankBox.DropItem(item);
            }
            ticket.FreeTicket = false;
        }
        
        public LottoTicketVendor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            Frozen = true;
		}
    }
}