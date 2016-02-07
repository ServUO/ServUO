using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Engines.Quests.Samurai
{
    public class HaochisTreasureChest : WoodenFootLocker
    {
        [Constructable]
        public HaochisTreasureChest()
        {
            this.Movable = false;

            this.GenerateTreasure();
        }

        public HaochisTreasureChest(Serial serial)
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

            if (player != null && player.Quest is HaochisTrialsQuest)
            {
                FifthTrialIntroObjective obj = player.Quest.FindObjective(typeof(FifthTrialIntroObjective)) as FifthTrialIntroObjective;

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

            if (player != null && player.Quest is HaochisTrialsQuest)
            {
                FifthTrialIntroObjective obj = player.Quest.FindObjective(typeof(FifthTrialIntroObjective)) as FifthTrialIntroObjective;

                if (obj != null)
                    obj.StolenTreasure = true;
            }

            Timer.DelayCall(TimeSpan.FromMinutes(2.0), new TimerCallback(GenerateTreasure));
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

            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(GenerateTreasure));
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