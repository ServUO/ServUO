using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Quests
{
    public class Gareth : MondainQuester
    {
        [Constructable]
        public Gareth()
            : base("Gareth", "the Emissary of the RBC")
        {
            m_NextTalk = DateTime.UtcNow;
        }

        public Gareth(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[] { typeof(TheQuestionsQuest) };

        public override void OnOfferFailed()
        {
            Say(1075787); // I feel that thou hast yet more to learn about Humility... Please ponder these things further, and visit me again on the 'morrow.
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Race = Race.Human;
            Body = 0x190;

            Hue = 0x83EA;
            HairItemID = 0x2049;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Boots());
            AddItem(new BodySash());
            AddItem(new FancyShirt(6));
            AddItem(new LongPants());
        }

        private DateTime m_NextTalk;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m_NextTalk < DateTime.UtcNow && m is PlayerMobile && m.Backpack != null && m.InRange(Location, 8))
            {
                PlayerMobile pm = (PlayerMobile)m;

                WhosMostHumbleQuest quest = QuestHelper.GetQuest(pm, typeof(WhosMostHumbleQuest)) as WhosMostHumbleQuest;

                if (quest != null)
                {
                    Item chain = pm.Backpack.FindItemByType(typeof(IronChain));

                    if (chain != null && chain.QuestItem)
                    {
                        SayTo(m, 1075773);
                        m_NextTalk = DateTime.UtcNow + TimeSpan.FromSeconds(10);
                    }
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

            m_NextTalk = DateTime.UtcNow;
        }
    }
}