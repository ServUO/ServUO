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
            InitStats(100, 100, 25);

            Hue = 0x83F2;

            Female = true;
            Body = 0x191;
            Name = "Alberta Giacco";
        }

        public override void InitOutfit()
        {
            AddItem(new FancyShirt());
            AddItem(new Skirt(0x59B));
            AddItem(new Boots());
            AddItem(new FeatheredHat(0x59B));
            AddItem(new FullApron(0x59B));

            HairItemID = 0x203D; // Pony Tail
            HairHue = 0x457;
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
                Direction = GetDirectionTo(player);

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

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}