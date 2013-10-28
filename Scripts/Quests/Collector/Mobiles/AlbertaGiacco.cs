using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Collector
{
    public class AlbertaGiacco : BaseQuester
    {
        [Constructable]
        public AlbertaGiacco()
            : base("the respected painter")
        {
        }

        public AlbertaGiacco(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Hue = 0x83F2;

            this.Female = true;
            this.Body = 0x191;
            this.Name = "Alberta Giacco";
        }

        public override void InitOutfit()
        {
            this.AddItem(new FancyShirt());
            this.AddItem(new Skirt(0x59B));
            this.AddItem(new Boots());
            this.AddItem(new FeatheredHat(0x59B));
            this.AddItem(new FullApron(0x59B));

            this.HairItemID = 0x203D; // Pony Tail
            this.HairHue = 0x457;
        }

        public override bool CanTalkTo(PlayerMobile to)
        {
            QuestSystem qs = to.Quest as CollectorQuest;

            if (qs == null)
                return false;

            return (qs.IsObjectiveInProgress(typeof(FindAlbertaObjective)) ||
                    qs.IsObjectiveInProgress(typeof(SitOnTheStoolObjective)) ||
                    qs.IsObjectiveInProgress(typeof(ReturnPaintingObjective)));
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            QuestSystem qs = player.Quest;

            if (qs is CollectorQuest)
            {
                this.Direction = this.GetDirectionTo(player);

                QuestObjective obj = qs.FindObjective(typeof(FindAlbertaObjective));

                if (obj != null && !obj.Completed)
                {
                    obj.Complete();
                }
                else if (qs.IsObjectiveInProgress(typeof(SitOnTheStoolObjective)))
                {
                    qs.AddConversation(new AlbertaStoolConversation());
                }
                else if (qs.IsObjectiveInProgress(typeof(ReturnPaintingObjective)))
                {
                    qs.AddConversation(new AlbertaAfterPaintingConversation());
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