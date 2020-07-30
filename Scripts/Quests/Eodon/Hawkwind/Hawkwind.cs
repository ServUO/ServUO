using Server.Mobiles;
using Server.Spells.SkillMasteries;

namespace Server.Engines.Quests.TimeLord
{
    public class Hawkwind : BaseQuester
    {
        [Constructable]
        public Hawkwind()
            : base("the Time Lord")
        {
        }

        public Hawkwind(Serial serial) : base(serial) { }

        public override bool CanBeDamaged() { return false; }
        public override bool ChangeRace => false;

        public override void InitBody()
        {
            Name = "Hawkwind";
            Body = 689;
            InitStats(100, 100, 100);
        }

        public override int GetAutoTalkRange(PlayerMobile pm)
        {
            return 3;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (from is PlayerMobile)
            {
                OnTalk((PlayerMobile)from, false);
            }
        }

        public override void OnTalk(PlayerMobile player, bool contextMenu)
        {
            if (player.Quest is TimeForLegendsQuest && ((TimeForLegendsQuest)player.Quest).Objectives.Count == 0)
            {
                player.CloseGump(typeof(ChooseMasteryGump));
                player.SendGump(new ChooseMasteryGump(player, (TimeForLegendsQuest)player.Quest));
            }
            else if (player.Quest == null && CanRecieveQuest(player))
            {
                Direction = GetDirectionTo(player);
                TimeForLegendsQuest quest = new TimeForLegendsQuest(player);
                quest.SendOffer();
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

        public bool CanRecieveQuest(Mobile m)
        {
            foreach (SkillName sk in MasteryInfo.Skills)
            {
                if (!m.Skills[sk].HasLearnedMastery())
                    return true;
            }

            return false;
        }
    }

}
