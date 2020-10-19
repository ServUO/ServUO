using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using System;
using System.Linq;

namespace Server.Engines.Quests
{
    public class JourneyToTheAthenaeumIsleQuest : BaseQuest
    {
        public override object Title => 1150929;         // Journey to the Athenaeum Isle

        public override object Description => 1150902;   /*Greetings, adventurer. <br><br>	As you know, my people have suffered the 
                                                                         * incessant onslaught of the Void and its minions for as long as Gargish 
                                                                         * history exists. Protecting Ter Mur from the darkness, and its desire to
                                                                         * consume the land completely, is a burden passed down from one ruler to another
                                                                         * upon ascension to the throne.  During my rule, I have been more successful 
                                                                         * than my predecessors but, now, I fear that the greatest evil both myself and 
                                                                         * my people have ever faced is about to return.<br><br>	Long ago, Ter Mur was
                                                                         * assaulted by the most formidable and horrid servant of the Void it had ever faced.
                                                                         * Called Scelestus the Defiler, this daemon proved invincible to any weapon or spell
                                                                         * that was utilized against him. I was unable to defeat him and was forced to 
                                                                         * imprison him instead. Sadly, my own daughter was caught in the spell and stands 
                                                                         * imprisoned next to the daemon. It has been this way for a thousand years now.
                                                                         * <br><br>	I have received word that the isle which houses the daemon, Athenaeum
                                                                         * Isle, is once again swarming with daemons. Based on the description provided to 
                                                                         * me, I believe these are the minions of the Defiler himself. They have no doubt 
                                                                         * crawled out of the dark in anticipation of their master’s return. In truth, the
                                                                         * prison I placed him within will not last forever.<br><br>	I ask that you journey
                                                                         * to the southwestern flight tower, adventurer, and head further southwest towards
                                                                         * the shore. Near the water's edge, you will find an ancient teleport site which 
                                                                         * will transport you to the isle. Once there, please slay as many of these monsters
                                                                         * as you can. Additionally, please keep your eye out for any documents that you may
                                                                         * discover. This isle was the former home of our Great Library and, when it fell, 
                                                                         * not all of the documents and books were able to be taken to the new location here 
                                                                         * in the Royal City.<br><br>	Slay the beasts and return to me any documents that 
                                                                         * you acquire.<br><br>	Be careful, and go with honor.*/

        public override object Refuse => 1150930;        // Understood. Perhaps you are not as brave as I initially thought. Be on your way, then.

        public override object Uncomplete => 1150931;    // You have returned. Did you manage to slay the beasts and obtain any documents that may be of interest?

        public override object Complete => 1150903;      /*You have returned! I cannot thank you enough for the service you have done me, 
                                                                         * adventurer. <br><br>	The documents that you have retrieved may seem unimportant 
                                                                         * to you, as they are naught but random letters and doctrines. But they each 
                                                                         * represent an echo of the past, musings of our ancestors. I had always meant to 
                                                                         * return to the former library and retrieve all that I could, but I had thought they
                                                                         * were safe, gathering dust in the ruins. I will immediately have these cleaned and
                                                                         * placed in the Great Library here in the Royal City.<br><br>	As thanks, I offer 
                                                                         * you this book. It is the chronicle of my life, of the arrival of the Defiler, and 
                                                                         * a history of my people. In hopes that you will be granted further understanding of
                                                                         * the impending danger we suffer, I offer it to you as a gesture of friendship and 
                                                                         * goodwill.<br><br>	Thank you again, on behalf of the Gargoyle people. I may have
                                                                         * need of your assistance at another time, should you be willing to come to my aid 
                                                                         * again.<br><br>	Until then, farewell.*/

        public JourneyToTheAthenaeumIsleQuest()
        {
            AddObjective(new SlayObjective(typeof(MinionOfScelestus), "Minion of Scelestus", 10));

            for (int i = 0; i < m_Types.Length; i++)
            {
                ObtainObjective obtain = new ObtainObjective(m_Types[i], m_Names[i], 1);

                AddObjective(obtain);
            }

            //AddObjective( new InternalObjective() );
            AddReward(new BaseReward(typeof(ChronicleOfTheGargoyleQueen1), 1, "Chronicle of the Gargoyle Queen Vol. I"));
        }

        public override bool RenderObjective(MondainQuestGump gump, bool offer)
        {
            int offset = 163;
            int page = 1;
            SlayObjective slay = Objectives.FirstOrDefault(o => o is SlayObjective) as SlayObjective;

            if (offer)
                gump.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                gump.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

            gump.AddHtmlObject(160, 70, 200, 40, Title, BaseQuestGump.DarkGreen, false, false);

            gump.AddPage(page);
            gump.AddButton(130, 430, 0x2EEF, 0x2EF1, 0, GumpButtonType.Page, page - 1);

            gump.AddHtmlLocalized(98, 147, 312, 16, 1072208, 0x2710, false, false); // All of the following	
            gump.AddHtmlLocalized(98, offset, 30, 16, 1072204, 0x15F90, false, false); // Slay	
            gump.AddLabel(133, offset, 0x481, "10   " + slay.Name); // %count% + %name%

            offset += 16;

            if (!offer)
            {
                gump.AddHtmlLocalized(103, offset, 120, 16, 3000087, 0x15F90, false, false); // Total			
                gump.AddLabel(223, offset, 0x481, slay.CurProgress.ToString());  // %current progress%

                offset += 16;
            }

            offset += 75;

            for (int i = 1; i < Objectives.Count; i++)
            {
                gump.AddHtmlLocalized(98, offset, 305, 16, 1150933 + (i - 1), 0x15F90, false, false);

                if (offset + 80 > 335)
                {
                    offset = 163;

                    gump.AddButton(275, 430, 0x2EE9, 0x2EEB, 0, GumpButtonType.Page, page + 1);
                    gump.AddPage(++page);
                    gump.AddButton(130, 430, 0x2EEF, 0x2EF1, 0, GumpButtonType.Page, page - 1);

                    if (i == Objectives.Count - 1)
                    {
                        RenderRewardPage(gump, offer);
                        break;
                    }
                }
                // render rewards page
                else if (i == Objectives.Count - 1)
                {
                    gump.AddButton(275, 430, 0x2EE9, 0x2EEB, 0, GumpButtonType.Page, page + 1);
                    gump.AddPage(++page);
                    RenderRewardPage(gump, offer);
                    gump.AddButton(130, 430, 0x2EEF, 0x2EF1, 0, GumpButtonType.Page, page - 1);
                    break;
                }
                else
                {
                    offset += 80;
                }
            }

            return true;
        }

        private void RenderRewardPage(MondainQuestGump gump, bool offer)
        {
            int offset = 163;

            if (offer)
                gump.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                gump.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log	

            gump.AddHtmlLocalized(98, 140, 312, 16, 1072201, 0x2710, false, false); // Reward	

            BaseReward reward = Rewards[0];

            gump.AddImage(105, offset, 0x4B9);
            gump.AddHtmlObject(133, offset, 280, 100, reward.Name, BaseQuestGump.LightGreen, false, false);
        }

        private readonly Type[] m_Types =
        {
                typeof(ChallengeRite),          typeof(AnthenaeumDecree),       typeof(LetterFromTheKing),
                typeof(OnTheVoid),              typeof(ShilaxrinarsMemorial),   typeof(ToTheHighScholar),
                typeof(ToTheHighBroodmother),   typeof(ReplyToTheHighScholar),  typeof(AccessToTheIsle),
                typeof(InMemory)
        };

        private readonly string[] m_Names =
        {
                "Obtain Gargish Document - Challenge Rite",             "Obtain Gargish Document - Athenaeum Decree",           "Obtain Gargish Document - Letter from the King",
                "Obtain Gargish Document - On the Void",                "Obtain Gargish Document - Shilaxrinar's Memorial",     "Obtain Gargish Document - To the High Scholar",
                "Obtain Gargish Document - To the High Broodmother",    "Obtain Gargish Document - Reply to the High Scholar",  "Obtain Gargish Document - Access to the Isle",
                "Obtain Gargish Document - In Memory"
        };
    }
}
