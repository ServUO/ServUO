using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests.Ninja
{
    public class EminosKatanaChest : WoodenChest
    {
        [Constructable]
        public EminosKatanaChest()
        {
            this.Movable = false;
            this.ItemID = 0xE42;

            this.GenerateTreasure();
        }

        public EminosKatanaChest(Serial serial)
            : base(serial)
        {
        }

        public override bool IsDecoContainer
        {
            get
            {
                return false;
            }
        }
        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player != null && player.InRange(this.GetWorldLocation(), 2))
            {
                QuestSystem qs = player.Quest;

                if (qs is EminosUndertakingQuest)
                {
                    if (EminosUndertakingQuest.HasLostEminosKatana(from))
                    {
                        Item katana = new EminosKatana();

                        if (!player.PlaceInBackpack(katana))
                        {
                            katana.Delete();
                            player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                        }
                    }
                    else
                    {
                        QuestObjective obj = qs.FindObjective(typeof(HallwayWalkObjective));

                        if (obj != null && !obj.Completed)
                        {
                            Item katana = new EminosKatana();

                            if (player.PlaceInBackpack(katana))
                            {
                                this.GenerateTreasure();
                                obj.Complete();
                            }
                            else
                            {
                                katana.Delete();
                                player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                            }
                        }
                    }
                }
            }

            base.OnDoubleClick(from);
        }

        public override bool CheckHold(Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight)
        {
            return false;
        }

        public override bool CheckItemUse(Mobile from, Item item)
        {
            return item == this;
        }

        public override bool CheckLift(Mobile from, Item item, ref LRReason reject)
        {
            if (from.AccessLevel >= AccessLevel.GameMaster)
                return true;

            PlayerMobile player = from as PlayerMobile;

            if (player != null && player.Quest is EminosUndertakingQuest)
            {
                HallwayWalkObjective obj = player.Quest.FindObjective(typeof(HallwayWalkObjective)) as HallwayWalkObjective;

                if (obj != null)
                {
                    if (obj.StolenTreasure)
                        from.SendLocalizedMessage(1063247); // The guard is watching you carefully!  It would be unwise to remove another item from here.
                    else
                        return true;
                }
            }

            return false;
        }

        public override void OnItemLifted(Mobile from, Item item)
        {
            PlayerMobile player = from as PlayerMobile;

            if (player != null && player.Quest is EminosUndertakingQuest)
            {
                HallwayWalkObjective obj = player.Quest.FindObjective(typeof(HallwayWalkObjective)) as HallwayWalkObjective;

                if (obj != null)
                    obj.StolenTreasure = true;
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }

        private void GenerateTreasure()
        {
            for (int i = this.Items.Count - 1; i >= 0; i--)
                this.Items[i].Delete();

            for (int i = 0; i < 75; i++)
            {
                switch ( Utility.Random(3) )
                {
                    case 0:
                        this.DropItem(new GoldBracelet());
                        break;
                    case 1:
                        this.DropItem(new GoldRing());
                        break;
                    case 2:
                        this.DropItem(Loot.RandomGem());
                        break;
                }
            }
        }
    }
}