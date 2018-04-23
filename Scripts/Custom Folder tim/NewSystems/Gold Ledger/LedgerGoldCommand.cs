//By: SHAMBAMPOW
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
//using Server.Engines.XmlSpawner2;

namespace Server.Commands
{
    public class LedgeGoldCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("LedgeGold", AccessLevel.Player, new CommandEventHandler(LedgeGold_OnCommand));
        }

        [Usage("LedgeGold")]
        [Description("Fills ledger with backpack gold.")]

        public static void LedgeGold_OnCommand(CommandEventArgs e)
        {
            //generate a list of all items in the backpack
            List<Item> packitems = RecurseFindItemsInPack(e.Mobile.Backpack);

            Item item = e.Mobile.Backpack.FindItemByType(typeof(GoldLedger));
            GoldLedger ledger = item as GoldLedger;

            if (ledger == null)
            {
                e.Mobile.SendMessage(33, "No gold ledger found.");
                return;
            }
            else
            {
                foreach (Item pitem in packitems)
                {
                    if (pitem is Gold)
                    {
                        Gold gold = pitem as Gold;

                        if (gold != null)
                        {
                            if (ledger.Gold < 999999999)
                            {
                                int golda = gold.Amount;
                                if ((gold.Amount + ledger.Gold) > 999999999)
                                    golda = (999999999 - ledger.Gold);
                                double maxgold = golda;
                                if (golda > maxgold)
                                    golda = (int)maxgold;
                                int GoldID = 0;
                                if (golda == 1)
                                    GoldID = gold.ItemID;
                                else if (golda > 1 && golda < 6)
                                    GoldID = gold.ItemID + 1;
                                else if (golda >= 6)
                                    GoldID = gold.ItemID + 2;
                                if (golda < gold.Amount)
                                    gold.Amount -= golda;
                                else
                                    gold.Delete();
                                ledger.Gold += golda;
                                if (ledger.b_open && golda > 0)
                                {
                                    e.Mobile.CloseGump(typeof(GoldLedgerGump));
                                    e.Mobile.SendGump(new GoldLedgerGump(ledger));
                                }
                                if (golda > 0)
                                {
                                    e.Mobile.SendMessage(2125, "You ledger the gold");
                                    Effects.SendMovingEffect(e.Mobile.Backpack, e.Mobile, GoldID, 5, 50, true, false);
                                    e.Mobile.PlaySound(0x2E6);
                                }
                            }
                        }
                    }
                }
            }
        }

        //this recursively checks a container for items, and tries to add them to the keys.  It will ignore
        //locked or trapped containers
        public static List<Item> RecurseFindItemsInPack(Container c)
        {
            List<Item> items = new List<Item>();

            foreach (Item item in c.Items)
            {
                //if the item is a container
                if (item is Container)
                {
                    //check if it can be trapped
                    if (item is TrapableContainer)
                    {
                        //if there's a trap on the container, ignore the container and move on
                        if (((TrapableContainer)item).TrapType != TrapType.None)
                        {
                            continue;
                        }

                        //if it's not trapped but is also lockable and is locked, ignore the container and move on
                        if (item is LockableContainer && ((LockableContainer)item).Locked)
                        {
                            continue;
                        }
                    }
                    //otherwise, recursively find items from this container
                    items.AddRange(RecurseFindItemsInPack((Container)item));
                }
                else	//it's not a container, so try to add to keys
                {
                    items.Add(item);
                }
            }
            return items;
        }
    }
}