using Server.Engines.Quests;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System;

namespace Server.Engines.Khaldun
{
    public class InspectorJasper : MondainQuester
    {
        public override Type[] Quests => null;

        public static InspectorJasper TramInstance { get; set; }

        public static void Initialize()
        {
            if (TramInstance == null)
            {
                TramInstance = new InspectorJasper();
                TramInstance.MoveToWorld(new Point3D(1675, 1584, 7), Map.Trammel);
                TramInstance.Direction = Direction.South;
            }
        }

        public InspectorJasper()
            : base("Jasper", "the Inspector")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;

            Race = Race.Elf;
            Hue = 33770;
            HairItemID = 0x2FCF;
            HairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            SetWearable(new LongPants(), 1156);
            SetWearable(new FancyShirt());
            SetWearable(new Epaulette(), 1156);
            SetWearable(new BodySash(), 1175);
            SetWearable(new Obi(), 1156);
            SetWearable(new Shoes(), 1910);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile mobile && mobile.InRange(Location, 5))
            {
                GoingGumshoeQuest quest = QuestHelper.GetQuest<GoingGumshoeQuest>(mobile);

                if (quest != null && quest.Completed)
                {
                    quest.GiveRewards();

                    BaseQuest newquest = QuestHelper.RandomQuest(mobile, new[] { typeof(GoingGumshoeQuest2) }, this);

                    if (newquest != null)
                        mobile.SendGump(new MondainQuestGump(newquest));
                }
                else
                {
                    GoingGumshoeQuest2 quest2 = QuestHelper.GetQuest<GoingGumshoeQuest2>(mobile);

                    if (quest2 != null)
                    {
                        if (quest2.IsComplete)
                        {
                            quest2.Objectives[0].CurProgress++;
                            quest2.GiveRewards(); // TODO: Does this quest end here?

                            BaseQuest newquest = QuestHelper.RandomQuest(mobile, new[] { typeof(GoingGumshoeQuest3) }, this);

                            if (newquest != null)
                                mobile.SendGump(new MondainQuestGump(newquest));
                        }
                        else
                        {
                            mobile.SendGump(new MondainQuestGump(quest2, MondainQuestGump.Section.InProgress, false));
                            quest2.InProgress();
                        }
                    }
                    else
                    {
                        GoingGumshoeQuest3 quest3 = QuestHelper.GetQuest<GoingGumshoeQuest3>(mobile);

                        if (quest3 != null)
                        {
                            if (quest3.IsComplete)
                            {
                                quest3.Objectives[0].CurProgress++;
                                quest3.GiveRewards(); // TODO: Does this quest end here?

                                BaseQuest newquest = QuestHelper.RandomQuest(mobile, new[] { typeof(GoingGumshoeQuest4) }, this);

                                if (newquest != null)
                                    mobile.SendGump(new MondainQuestGump(newquest));
                            }
                            else
                            {
                                mobile.SendGump(new MondainQuestGump(quest3, MondainQuestGump.Section.InProgress, false));
                                quest3.InProgress();
                            }
                        }
                        else
                        {
                            GoingGumshoeQuest4 quest4 = QuestHelper.GetQuest<GoingGumshoeQuest4>(mobile);

                            if (quest4 != null && !quest4.IsComplete)
                            {
                                mobile.SendGump(new MondainQuestGump(quest4, MondainQuestGump.Section.InProgress, false));
                                quest4.InProgress();
                            }
                            else if (quest4 == null)
                            {
                                SayTo(mobile, 1080107); // I'm sorry, I have nothing for you at this time.
                            }
                        }
                    }
                }
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is PlayerMobile mobile && InLOS(mobile) && InRange(mobile.Location, 3) && !InRange(oldLocation, 3))
            {
                GoingGumshoeQuest quest = QuestHelper.GetQuest<GoingGumshoeQuest>(mobile);

                if (quest != null)
                {
                    quest.Objectives[0].CurProgress++;
                    quest.OnCompleted();
                }
                else
                {
                    GoingGumshoeQuest4 quest2 = QuestHelper.GetQuest<GoingGumshoeQuest4>(mobile);

                    if (quest2 != null && quest2.IsComplete)
                    {
                        quest2.Objectives[0].CurProgress++;

                        mobile.SendGump(new InternalGump());
                        mobile.PlaySound(quest2.CompleteSound);
                        quest2.GiveRewards();
                    }
                }
            }
        }

        private class InternalGump : Gump
        {
            public InternalGump()
                : base(50, 50)
            {
                AddBackground(0, 0, 400, 600, 9300);
                AddImage(58, 30, 1745);

                AddHtmlLocalized(0, 340, 400, 20, 1154645, "#1158625", 0x0, false, false); // It all comes together...
                AddHtmlLocalized(5, 365, 390, 200, 1158626, BaseGump.C32216(0x0D0D0D), false, true);

                /**You approach Inspector Jasper and relay to him what Sage Humbolt told you...he remains expressionless*
                 * This is much more sinister than any of us could have ever imagined. I had little doubt your investigatory 
                 * skills would yield anything but a major revelation - but this? Who could have thought. I have dispatched
                 * a team to the location Sage Humbolt spoke of. It is up to everyone now to prevent this coming evil. Here
                 * you go, you've earned this. Wear this title proudly. This official credential identifies you as a member 
                 * of the RBG Detective Branch and will allow you past the guards at the site Sage Humbolt spoke of near
                 * 57o 7'S, 5o 20'E in the Lost Lands.*/
            }
        }

        public InspectorJasper(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }
        }
    }
}
