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
            AddItem(new Backpack());
            AddItem(new Shoes(1819));
            AddItem(new LeatherArms());
            AddItem(new LeatherChest());
            AddItem(new LeatherLegs());
            AddItem(new LeatherGloves());
            AddItem(new GnarledStaff());
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
