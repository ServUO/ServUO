using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Collector
{
    public class TomasONeerlan : BaseQuester
    {
        [Constructable]
        public TomasONeerlan()
            : base("the famed toymaker")
        {
        }

        public TomasONeerlan(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = 0x83F8;

            this.Female = false;
            this.Body = 0x190;
            this.Name = "Tomas O'Neerlan";
        }

        public override void InitOutfit()
        {
            this.AddItem(new FancyShirt());
            this.AddItem(new LongPants(0x546));
            this.AddItem(new Boots(0x452));
            this.AddItem(new FullApron(0x455));

            this.HairItemID = 0x203B;	//ShortHair
            this.HairHue = 0x455;
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            QuestSystem qs = to.Quest as CollectorQuest;

            if (qs == null)
                return false;

            return (qs.IsObjectiveInProgress(typeof(FindTomasObjective)) ||
                    qs.IsObjectiveInProgress(typeof(CaptureImagesObjective)) ||
                    qs.IsObjectiveInProgress(typeof(ReturnImagesObjective)));
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            QuestSystem qs = player.Quest;

            if (qs is CollectorQuest)
            {
                this.Direction = this.GetDirectionTo(player);

                QuestObjective obj = qs.FindObjective(typeof(FindTomasObjective));

                if (obj != null && !obj.Completed)
                {
                    Item paints = new EnchantedPaints();

                    if (!player.PlaceInBackpack(paints))
                    {
                        paints.Delete();
                        player.SendLocalizedMessage(1046260); // You need to clear some space in your inventory to continue with the quest.  Come back here when you have more space in your inventory.
                    }
                    else
                    {
                        obj.Complete();
                    }
                }
                else if (qs.IsObjectiveInProgress(typeof(CaptureImagesObjective)))
                {
                    qs.AddConversation(new TomasDuringCollectingConversation());
                }
                else
                {
                    obj = qs.FindObjective(typeof(ReturnImagesObjective));

                    if (obj != null && !obj.Completed)
                    {
                        if (player.Backpack != null)
                            player.Backpack.ConsumeUpTo(typeof(EnchantedPaints), 1);

                        obj.Complete();
                    }
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}