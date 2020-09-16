using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Prompts;
using System;
using System.Collections.Generic;

namespace Server.Engines.NewMagincia
{
    public class WarehouseSuperintendent : BaseCreature
    {
        public override bool IsInvulnerable => true;

        [Constructable]
        public WarehouseSuperintendent() : base(AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 2)
        {
            Race = Race.Human;
            Blessed = true;
            Title = "The Warehouse Superintendent";

            if (Utility.RandomBool())
            {
                Female = true;
                Body = 0x191;
                Name = NameList.RandomName("female");

                AddItem(new Skirt(Utility.RandomPinkHue()));
            }
            else
            {
                Female = false;
                Body = 0x190;
                Name = NameList.RandomName("male");
                AddItem(new ShortPants(Utility.RandomBlueHue()));
            }

            AddItem(new Tunic(Utility.RandomBlueHue()));
            AddItem(new Boots());

            Utility.AssignRandomHair(this, Utility.RandomHairHue());
            Utility.AssignRandomFacialHair(this, Utility.RandomHairHue());

            Hue = Race.RandomSkinHue();
        }

        /*public override void OnDoubleClick(Mobile from)
		{
			if(from.InRange(this.Location, 3) && from.Backpack != null)
			{
				WarehouseContainer container = MaginciaBazaar.ClaimContainer(from);
				
				if(container != null)
					TryTransferItems(from, container);
			}
		}*/

        public void TryTransfer(Mobile from, StorageEntry entry)
        {
            if (entry == null)
                return;

            int fees = entry.Funds;

            if (fees < 0)
            {
                int owed = fees * -1;
                SayTo(from, string.Format("It looks like you owe {0}gp as back fees. How much would you like to pay now?", owed.ToString("###,###,###")));
                from.Prompt = new BackfeePrompt(this, entry);
                return;
            }

            if (!TryPayFunds(from, entry))
            {
                from.SendGump(new BazaarInformationGump(1150681, 1150678)); // Some personal possessions that were equipped on the broker still remain in storage, because your backpack cannot hold them. Please free up space in your backpack and return to claim these items.
                return;
            }

            if (entry.Creatures.Count > 0)
            {
                List<BaseCreature> list = new List<BaseCreature>(entry.Creatures);

                foreach (BaseCreature bc in list)
                {
                    if (from.Stabled.Count < AnimalTrainer.GetMaxStabled(from))
                    {
                        bc.Blessed = false;
                        bc.ControlOrder = OrderType.Stay;
                        bc.Internalize();
                        bc.IsStabled = true;
                        bc.Loyalty = MaxLoyalty; // Wonderfully happy
                        from.Stabled.Add(bc);
                        bc.SetControlMaster(null);
                        bc.SummonMaster = null;

                        entry.RemovePet(bc);
                    }
                    else
                    {
                        from.SendGump(new BazaarInformationGump(1150681, 1150678)); // Some personal possessions that were equipped on the broker still remain in storage, because your backpack cannot hold them. Please free up space in your backpack and return to claim these items.
                        return;
                    }
                }

                ColUtility.Free(list);
            }

            if (entry.CommodityTypes.Count > 0)
            {
                Dictionary<Type, int> copy = new Dictionary<Type, int>(entry.CommodityTypes);

                foreach (KeyValuePair<Type, int> commodities in copy)
                {
                    Type type = commodities.Key;
                    int amt = commodities.Value;

                    if (!GiveItems(from, type, amt, entry))
                    {
                        from.SendGump(new BazaarInformationGump(1150681, 1150678)); // Some personal possessions that were equipped on the broker still remain in storage, because your backpack cannot hold them. Please free up space in your backpack and return to claim these items.
                        return;
                    }
                }

                copy.Clear();
            }

            entry.CommodityTypes.Clear();
            ColUtility.Free(entry.Creatures);

            from.SendGump(new BazaarInformationGump(1150681, 1150677)); // There are no longer any items or funds in storage for your former bazaar stall. Thank you for your diligence in recovering your possessions.
            MaginciaBazaar.RemoveFromStorage(from);
        }

        private bool GiveItems(Mobile from, Type type, int amt, StorageEntry entry)
        {
            int amount = amt;

            while (amount > 60000)
            {
                CommodityDeed deed = new CommodityDeed();
                Item item = Loot.Construct(type);
                item.Amount = 60000;
                deed.SetCommodity(item);
                amount -= 60000;

                if (from.Backpack == null || !from.Backpack.TryDropItem(from, deed, false))
                {
                    deed.Delete();
                    return false;
                }
                else
                    entry.RemoveCommodity(type, 60000);
            }

            CommodityDeed deed2 = new CommodityDeed();
            Item item2 = Loot.Construct(type);
            item2.Amount = amount;
            deed2.SetCommodity(item2);

            if (from.Backpack == null || !from.Backpack.TryDropItem(from, deed2, false))
            {
                deed2.Delete();
                return false;
            }
            else
                entry.RemoveCommodity(type, amount);

            return true;
        }

        private bool TryPayFunds(Mobile from, StorageEntry entry)
        {
            int amount = entry.Funds;

            if (Banker.Withdraw(from, amount, true))
            {
                entry.Funds = 0;
                return true;
            }

            return false;
        }

        public void TryPayBackfee(Mobile from, string text, StorageEntry entry)
        {
            int amount = Utility.ToInt32(text);
            int owed = entry.Funds * -1;

            if (amount > 0)
            {
                int toDeduct = Math.Min(owed, amount);

                if (Banker.Withdraw(from, toDeduct))
                {
                    entry.Funds += toDeduct;
                    int newAmount = entry.Funds;

                    if (newAmount >= 0)
                    {
                        TryTransfer(from, entry);
                    }
                    else
                    {
                        SayTo(from, string.Format("Thank you! You have a remaining balance of {0}gp as backfees!", newAmount * -1));
                    }
                }
                else
                {
                    SayTo(from, "You don't have enough funds in your bankbox to support that amount.");
                }
            }

        }

        private class BackfeePrompt : Prompt
        {
            private readonly WarehouseSuperintendent m_Mobile;
            private readonly StorageEntry m_Entry;

            public BackfeePrompt(WarehouseSuperintendent mobile, StorageEntry entry)
            {
                m_Mobile = mobile;
                m_Entry = entry; ;
            }

            public override void OnResponse(Mobile from, string text)
            {
                m_Mobile.TryPayBackfee(from, text, m_Entry);
            }

        }

        /*private bool TransferItems(Mobile from, WarehouseContainer c)
		{
			List<Item> items = new List<Item>(c.Items);
			
			foreach(Item item in items)
			{
				from.Backpack.TryDropItem(from, item, false);
			}
			
			return c.Items.Count == 0;
		}*/

        public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);

            list.Add(new ClaimStorageEntry(from, this));
            list.Add(new ChangeMatchBidEntry(from));
        }

        private class ClaimStorageEntry : ContextMenuEntry
        {
            private readonly WarehouseSuperintendent m_Mobile;
            private readonly StorageEntry m_Entry;

            public ClaimStorageEntry(Mobile from, WarehouseSuperintendent mobile) : base(1150681, 3)
            {
                m_Mobile = mobile;
                m_Entry = MaginciaBazaar.GetStorageEntry(from);

                if (m_Entry == null)
                    Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                Mobile from = Owner.From;

                if (from == null || m_Entry == null)
                    return;

                m_Mobile.TryTransfer(from, m_Entry);
            }
        }

        private class ChangeMatchBidEntry : ContextMenuEntry
        {
            public ChangeMatchBidEntry(Mobile from) : base(1150587, 3)
            {
            }

            public override void OnClick()
            {
                Mobile from = Owner.From;

                if (from != null)
                    from.SendGump(new MatchBidGump(from, null));
            }
        }

        public WarehouseSuperintendent(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            if (version == 0)
            {
                Hue = Race.RandomSkinHue();
            }
        }
    }
}