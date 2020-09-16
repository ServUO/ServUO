using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Services.TownCryer;

namespace Server.Engines.Quests
{
    public class WishesOfTheWispQuest : BaseQuest
    {
        /*Wishes of Wisps*/
        public override object Title => 1158296;

        /*The story of the brothers Andros and Adrian is troubling, yet fascinates you. You have heard rumor of items 
         * traded by the mysterious wisps. Despite the dangers you decide you should venture to the dungeon Despise.*/
        public override object Description => 1158318;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Visit the Dungeon Despise and investigate. */
        public override object Uncomplete => 1158297;

        //public override object Complete { get { return 1158378; } }

        public override int CompleteMessage => 1156585;  // You've completed a quest!

        public override int AcceptSound => 0x2E8;
        public override bool DoneOnce => true;

        public WishesOfTheWispQuest()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1158298)); // A step closer to understanding what happened at Despise
        }

        public void CompleteQuest()
        {
            OnCompleted();
            Objectives[0].CurProgress++;
            TownCryerSystem.CompleteQuest(Owner, 1153468, 1158309, 0x65C);
            GiveRewards();
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription => Quest.Uncomplete;

            public InternalObjective()
                : base(1)
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

                int version = reader.ReadInt();
            }
        }
    }

    public class WhisperingWithWispsQuest : BaseQuest
    {
        /*Whispering with Wisps*/
        public override object Title => 1158300;

        /*The mysterious wisp seems friendly. You are taken by the mysterious creature and it's iridescent glow. The wisp directs
         * you inside the dungeon, but otherwise does not respond to your presence. You feel guided by your karma. Entering the
         * dungeon with negative karma will draw you to the depths of despise while entering with positive karma will draw you up
         * into the peaceful glades above. You have learned from the wisp you must venture inside the dungeon and seek an Ankh.*/
        public override object Description => 1158301;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Enter the appropriate level of despise based on your karma. Once inside, find and use an ankh. */
        public override object Uncomplete => 1158302;

        /*You have successfully freed Despise from the eternal feud between Andros and Adrian. Despite your efforts, you no doubt
        * believe their strong magics will compel them to battle once again. You rejoice, however, in your small albeit short
        * lived victory! The wisp seems eternally grateful and grants you a generous gift!*/
        public override object Complete => 1158323;

        public override int CompleteMessage => 1158322;
        // You have successfully slayed the brother and freed Despise from their eternal feud! Return to the Wisp outside the dungeon to claim your reward!

        public override int AcceptSound => 0x2E8;
        public override bool DoneOnce => true;

        public WhisperingWithWispsQuest()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(typeof(DespiseTitleDeed), 1158139));
        }

        public override bool RenderObjective(MondainQuestGump g, bool offer)
        {
            if (offer)
                g.AddHtmlLocalized(130, 45, 270, 16, 1049010, 0xFFFFFF, false, false); // Quest Offer
            else
                g.AddHtmlLocalized(130, 45, 270, 16, 1046026, 0xFFFFFF, false, false); // Quest Log

            g.AddButton(130, 430, 0x2EEF, 0x2EF1, (int)Buttons.PreviousPage, GumpButtonType.Reply, 0);
            g.AddButton(275, 430, 0x2EE9, 0x2EEB, (int)Buttons.NextPage, GumpButtonType.Reply, 0);

            g.AddHtmlObject(160, 70, 200, 40, Title, BaseQuestGump.DarkGreen, false, false);
            g.AddHtmlLocalized(98, 140, 312, 16, 1049073, 0x2710, false, false); // Objective:

            g.AddHtmlLocalized(98, 156, 312, 16, 1072208, 0x2710, false, false); // All of the following	

            g.AddHtmlLocalized(98, 172, 312, 83, 1158302, BaseQuestGump.LightGreen, false, false);
            /* Enter the appropriate level of despise based on your karma. Once inside, find and use an ankh.*/

            g.AddHtmlLocalized(98, 255, 312, 40, 1158305, BaseQuestGump.LightGreen, false, false);
            //Using your wisp, posses a creature within the dungeon.

            g.AddHtmlLocalized(98, 335, 312, 40, 1158306, BaseQuestGump.LightGreen, false, false);
            // Defeat Andros or Adrian in the depths of Despise.

            return true;
        }

        public static void OnBossSlain(Despise.DespiseBoss boss)
        {
            foreach (DamageStore ds in boss.GetLootingRights())
            {
                if (ds.m_Mobile is PlayerMobile)
                {
                    PlayerMobile pm = ds.m_Mobile as PlayerMobile;
                    WhisperingWithWispsQuest quest = QuestHelper.GetQuest<WhisperingWithWispsQuest>(pm);

                    if (quest != null && !quest.Completed)
                    {
                        quest.Objectives[0].CurProgress++;
                        quest.OnCompleted();
                    }
                }
            }
        }

        public void CompleteQuest()
        {
            TownCryerSystem.CompleteQuest(Owner, 1158303, 1158323, 0x650);
            GiveRewards();

            Owner.SendLocalizedMessage(1158326); // For your accomplishments you have been awarded a bonus 1000 Despise points! Trade with the wisp to redeem them!
            Points.PointsSystem.DespiseCrystals.AwardPoints(Owner, 1000, false, false);
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription => Quest.Uncomplete;

            public InternalObjective()
                : base(1)
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

                int version = reader.ReadInt();
            }
        }
    }
}