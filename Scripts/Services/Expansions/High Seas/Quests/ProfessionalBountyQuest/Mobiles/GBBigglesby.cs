using Server;
using System;
using Server.Engines.Quests;
using Server.Multis;
using Server.Items;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class GBBigglesby : MondainQuester
    {
        public override Type[] Quests { get { return new Type[] { typeof(ProfessionalBountyQuest) }; } }

        [Constructable]
        public GBBigglesby()
        {
            Title = "the proprietor";
            Name = "G.B. Bigglesby";
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);
            Female = false;
            Race = Race.Human;

            Hue = Race.RandomSkinHue();
            Race.RandomHair(this);
            HairHue = Race.RandomHairHue();

            Item fancyShirt = new FancyShirt();
            Item shirt = new Shirt(PirateCaptain.GetRandomShirtHue());
            shirt.Layer = Layer.OuterTorso;

            AddItem(new Cloak(5));
            AddItem(new Cutlass());
            AddItem(shirt);
            AddItem(fancyShirt);
            AddItem(new LongPants());
            AddItem(new Boots());

            m_NextSay = DateTime.UtcNow;
        }

        private int m_LastSay;
        private DateTime m_NextSay;

        public override void OnTalk(PlayerMobile pm)
        {
            if (!HasQuest(pm))
            {
                BaseBoat boat = FishQuestHelper.GetBoat(pm);

                if (boat != null && boat is BaseGalleon)
                {
                    if (((BaseGalleon)boat).Scuttled)
                    {
                        pm.SendLocalizedMessage(1116752); //Your ship is a mess!  Fix it first and then we can talk about catching pirates.
                    }
                    else
                    {
                        ProfessionalBountyQuest q = new ProfessionalBountyQuest((BaseGalleon)boat);
                        q.Owner = pm;
                        q.Quester = this;

                        pm.CloseGump(typeof(MondainQuestGump));
                        pm.SendGump(new MondainQuestGump(q));
                    }
                }
                else if (boat != null && !(boat is BaseGalleon))
                {
                    SayTo(pm, 1116751); //The ship you are captaining could not take on a pirate ship.  Bring a warship if you want this quest.
                }
                else if(m_NextSay < DateTime.UtcNow)
                {
                    if (m_LastSay == 0)
                    {
                        if (this.Map != Map.Tokuno)
                            Say(1152651);  //I'm G.B. Bigglesby, proprietor of the G.B. Bigglesby Free Trade Floating Emporium.
                        else
                            Say("I am {0}, proprietor of {0} Free Trade Coroporation of Tokuno.", Name);
                        m_LastSay = 1;
                    }
                    else
                    {
                        Say(1152652);  //This sea market be me life's work and 'tis me pride and joy..
                        m_LastSay = 0;
                    }

                    m_NextSay = DateTime.UtcNow + TimeSpan.FromSeconds(5);
                }
            }
        }

        public override void Advertise()
        {
        }

        public bool HasQuest(PlayerMobile pm)
        {
            if (pm.Quests == null)
                return false;

            for (int i = 0; i < pm.Quests.Count; i++)
            {
                BaseQuest quest = pm.Quests[i];

                if (quest is ProfessionalBountyQuest)
                {
                    if (this == quest.Quester)
                    {
                        for (int j = 0; j < quest.Objectives.Count; j++)
                        {
                            if (quest.Objectives[j].Update(pm))
                                quest.Objectives[j].Complete();
                        }
                    }

                    if (quest.Completed)
                    {
                        quest.OnCompleted();
                        pm.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.Complete, false, true));
                    }
                    else
                    {
                        pm.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.InProgress, false));
                        quest.InProgress();
                    }

                    return true;
                }
            }
            return false;
        }

        public GBBigglesby(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_NextSay = DateTime.UtcNow;
        }
    }
}