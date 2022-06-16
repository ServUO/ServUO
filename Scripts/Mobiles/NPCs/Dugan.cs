using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class Dugan : MondainQuester
    {
        [Constructable]
        public Dugan()
            : base("Elder Dugan", "the Prospector")
        {
        }

        public Dugan(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
        {
            typeof(Missing)
        };

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Race = Race.Human;
            Body = 0x190;

            Hue = 0x83EA;
            HairItemID = 0x203C;
            CantWalk = true;

            Direction = Direction.Left;
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile pm && m.InRange(Location, 5))
            {
                if (QuestHelper.CheckDoneOnce(pm, typeof(Missing), null, false))
                {
                    if (QuestHelper.CheckDoneOnce(pm, typeof(EscortToDugan), null, false))
                    {
                        var q = QuestHelper.GetQuest<EndingtheThreat>(pm);

                        if (q == null)
                        {
                            var quest = QuestHelper.RandomQuest(pm, new Type[] { typeof(EndingtheThreat) }, this);

                            if (quest != null)
                            {
                                pm.CloseGump(typeof(MondainQuestGump));
                                pm.SendGump(new MondainQuestGump(quest));
                            }
                        }
                        else
                        {
                            OnTalk(pm);
                        }
                    }
                    else
                    {
                        OnOfferFailed();
                    }
                }
                else
                {
                    OnTalk(pm);
                }
            }
        }

        public override void InitOutfit()
        {
            SetWearable(new Backpack());
            SetWearable(new Shoes(), 1819, 1);
            SetWearable(new LeatherArms(), dropChance: 1);
            SetWearable(new LeatherChest(), dropChance: 1);
            SetWearable(new LeatherLegs(), dropChance: 1);
            SetWearable(new LeatherGloves(), dropChance: 1);
            SetWearable(new GnarledStaff(), dropChance: 1);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
