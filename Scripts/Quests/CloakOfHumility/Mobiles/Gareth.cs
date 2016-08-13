using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{
    public class Gareth : MondainQuester
    {
        [Constructable]
        public Gareth()
            : base("Gareth", "the Emissary of the RBC")
        {
            m_NextTalk = DateTime.Now;
        }

        public Gareth(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(TheQuestionsQuest),
                };
            }
        }

        public override void InitBody()
        {
            this.InitStats(100, 100, 25);

            this.Female = false;
            this.Race = Race.Human;
            this.Body = 0x190;

            this.Hue = 0x83EA;
            this.HairItemID = 0x2049;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Boots());
            this.AddItem(new BodySash());
            this.AddItem(new FancyShirt(6));
            this.AddItem(new LongPants());
        }

        private DateTime m_NextTalk;

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m_NextTalk < DateTime.Now && m is PlayerMobile && m.Backpack != null && m.InRange(this.Location, 8))
            {
                PlayerMobile pm = (PlayerMobile)m;

                WhosMostHumbleQuest quest = QuestHelper.GetQuest(pm, typeof(WhosMostHumbleQuest)) as WhosMostHumbleQuest;

                if (quest != null)
                {
                    Item chain = pm.Backpack.FindItemByType(typeof(IronChain));

                    if (chain != null && chain.QuestItem)
                    {
                        SayTo(m, 1075773);
                        m_NextTalk = DateTime.Now + TimeSpan.FromSeconds(10);
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

            m_NextTalk = DateTime.Now;
        }
    }
}