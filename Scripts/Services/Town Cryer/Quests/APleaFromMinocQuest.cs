using Server.Items;
using Server.Mobiles;
using Server.Services.TownCryer;
using System;

namespace Server.Engines.Quests
{
    public class APleaFromMinocQuest : BaseQuest
    {
        /* A Plea from Minoc */
        public override object Title => 1158259;

        /*The Governor of Minoc has made a plea to any and all of those willing and able to come to defense of the City. 
         * Cora the Sorcerers has overtaken the Dungeon Covetous and corrupted the creatures that reside within. You hear 
         * rumors the Governor has authorized the Sheriff of Minoc to bestow great fortune and fame to those who sacrifice
         * in the name of the city.*/
        public override object Description => 1158260;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Visit the Skara Brae Ranger's Guild and participate in the Huntmaster's Challenge */
        public override object Uncomplete => 1158261;

        /* You have braved the wilds of Britannia and slayed a mighty beast! You have meticulously documented your kill 
         * and submitted it to the Ranger's Guild for evaluation. If luck was on your side you may indeed have 
         * the largest quarry for the month...or maybe not. Alas, your bravery has earned you the well deserved title
         * of Hunter! May you go fearlessly into the wilderness in search of your next big kill! Well done! */
        public override object Complete => 1158378;

        public override int CompleteMessage => 1156585;  // You've completed a quest!

        public override int AcceptSound => 0x2E8;
        public override bool DoneOnce => true;

        public APleaFromMinocQuest()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1158262)); // A Reward Title Deed
        }

        public void CompleteQuest()
        {
            OnCompleted();
            Objectives[0].Complete();
            TownCryerSystem.CompleteQuest(Owner, 1158275, 1158276, 0x65B);
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

    public class ClearingCovetousQuest : BaseQuest
    {
        /* Clearing Covetous */
        public override object Title => 1158263;

        /*That's right, I'm the Sheriff of Minoc, what can I help you with citizen? What you read in the Town Cryer is true,
         * Covetous has become quite dangerous. It seems a vile Sorcerers called Cora has overtaken the dungeon and corrupted 
         * the creatures within, binding them to her cruel rule. The Lycaeum had been trying to contain her magics within 
         * the dungeon with something called the "Void Pool" but the mage they sent has not been seen in quite some time. 
         * The place is dangerous to say the least and requires skilled combatants who will encounter greater success if 
         * they pool their resources. None the less, the Governor has authorized me to deputize any and all who sacrifice
         * on behalf of Minoc and attempt to cleanse Covetous. As you know the mountain is a key strategic resource to 
         * valuable ore that is vital to Minoc's economy. Prove yourself to the City and you shall not soon be forgotten...*/
        public override object Description => 1158264;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Go to the dungeon Covetous Level 1 and defeat the creatures within! */
        public override object Uncomplete => 1158266;

        /* Indeed, you have proven yourself, and with your clearing of the creatures in the upper levels of Covetous Mountain our 
         * miners can once again return to their normal operations, ensuring the lifeblood of our city is one again flowing. You 
         * are no doubt brave and strong, but your next task will test your endurance no doubt. As I said, the Lycaeum is keeping
         * Cora's power at bay with something called the Void Pool. The magics prevent Cora herself from destroying it, but her 
         * minions are not bound by that restriction. Only blade and spell can defeat her forces as they try to destroy the Void 
         * Pool. Defend the Void Pool at all costs and sacrifice for Minoc. Do this and you will no doubt be remembered a hero. */
        //public override object Complete { get { return 1158268; } }

        public override int CompleteMessage => 1158267;  // You've cleared enough creatures to allow the miners of 
                                                         // Minoc to return to their mining operations. Return to the Sheriff and report the news.

        public override int AcceptSound => 0x2E8;
        public override bool DoneOnce => true;

        public ClearingCovetousQuest()
        {
            AddObjective(new SlayObjective(typeof(HeadlessMiner), "headless miners", 40, "Covetous"));
            AddObjective(new SlayObjective(typeof(VampireMongbat), "vampire mongbats", 30, "Covetous"));
            AddObjective(new SlayObjective(typeof(DazzledHarpy), "dazzled harpies", 20, "Covetous"));
            AddObjective(new SlayObjective(typeof(StrangeGazer), "strange gazers", 10, "Covetous"));

            AddReward(new BaseReward(1158265)); // A step closer to glory for thy deeds...
        }

        public override void OnCompleted()
        {
            base.OnCompleted();

            GiveRewards();
        }

        /*public void CompleteChallenge()
        {
            OnCompleted();
            Objectives[0].CurProgress++;
            TownCryerSystem.CompleteQuest(Owner, 1158275, 1158276, 0x65B);
            GiveRewards();
        }*/
    }

    public class AForcedSacraficeQuest : BaseQuest
    {
        /* A Forced Sacrifice */
        public override object Title => 1158271;

        /* Indeed, you have proven yourself, and with your clearing of the creatures in the upper levels of Covetous Mountain our 
         * miners can once again return to their normal operations, ensuring the lifeblood of our city is one again flowing. You 
         * are no doubt brave and strong, but your next task will test your endurance no doubt. As I said, the Lycaeum is keeping
         * Cora's power at bay with something called the Void Pool. The magics prevent Cora herself from destroying it, but her 
         * minions are not bound by that restriction. Only blade and spell can defeat her forces as they try to destroy the Void 
         * Pool. Defend the Void Pool at all costs and sacrifice for Minoc. Do this and you will no doubt be remembered a hero. */
        public override object Description => 1158268;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Go to the Void Pool located in Level 2 of Covetous and defend it from Cora's armies! */
        public override object Uncomplete => 1158269;

        //public override object Complete { get { return 1158268; } }

        /*You have defended the void pool until your last breath, your sacrifice for Minoc will not be soon forgotten! Return to
         * the Sheriff of Minoc and report the news!*/
        public override int CompleteMessage => 1158270;

        public override int AcceptSound => 0x2E8;
        public override bool DoneOnce => true;

        public AForcedSacraficeQuest()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1158265)); // A step closer to glory for thy deeds...
        }

        public void CompleteQuest()
        {
            OnCompleted();
            Objectives[0].CurProgress++;

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

    public class AForcedSacraficeQuest2 : BaseQuest
    {
        /* A Forced Sacrifice */
        public override object Title => 1158271;

        /* Despite your failure in protecting the void pool, your efforts have allowed the mages of the Lycaeum to use their
         * magics and bind the void pool in an infinite time loop, forever sealing Cora within the dungeon. This is only a
         * stopgap measure, however, and Cora cannot be allowed to continue her twisted craft. Now comes the ultimate test,
         * you must venture to the deepest level of Dungeon Covetous and slay Cora. It is the only way. Take this, it is all 
         * the city can offer you in an effort to slay Cora. The City is counting on you. */
        public override object Description => 1158272;

        /* You decide against accepting the quest. */
        public override object Refuse => 1158130;

        /* Go to the furthest depths of Covetous and slay Cora! */
        public override object Uncomplete => 1158273;

        /*You have slayed the vile sorceress Cora! A powerful mage as she was, her armies will no doubt attempt
        * to resurrect their general - for now though Minoc is safe. The economic future of Minoc has been 
        * secured and for your efforts you are hereby bestowed a great honor!*/
        public override object Complete => 1158281;

        /*You have slayed the sorceress Cora!  Return to the Sheriff of Minoc and report the news!*/
        public override int CompleteMessage => 1158274;

        public override int AcceptSound => 0x2E8;
        public override bool DoneOnce => true;

        public AForcedSacraficeQuest2()
        {
            AddObjective(new SlayObjective(typeof(CoraTheSorceress), "Cora the Sorcerer", 1, "Covetous"));

            AddReward(new BaseReward(typeof(HeroOfMincRewardTitleDeed), 1158139)); // A Reward Title Deed
        }

        public override void OnAccept()
        {
            base.OnAccept();

            if (QuestHelper.TryReceiveQuestItem(Owner, typeof(MysteriousPotion), TimeSpan.FromDays(3)))
            {
                Owner.AddToBackpack(new MysteriousPotion());
            }
        }

        public void CompleteQuest()
        {
            TownCryerSystem.CompleteQuest(Owner, 1158280, 1158281, 0x623);
            GiveRewards();

            Points.PointsSystem.VoidPool.AwardPoints(Owner, 2000, false, false);
            Owner.SendLocalizedMessage(1158282); // For your accomplishments you have been awarded a bonus 2000 Covetous points! Visit Vela in the Town of Cove to redeem them!
        }
    }

    public class SheriffOfMinoc : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(ToolsOfTheTradeQuest) };

        public static SheriffOfMinoc TramInstance { get; set; }
        public static SheriffOfMinoc FelInstance { get; set; }

        public static void Initialize()
        {
            if (TramInstance == null)
            {
                TramInstance = new SheriffOfMinoc();
                TramInstance.MoveToWorld(new Point3D(2462, 439, 15), Map.Trammel);
                TramInstance.Direction = Direction.South;
            }

            if (FelInstance == null)
            {
                FelInstance = new SheriffOfMinoc();
                FelInstance.MoveToWorld(new Point3D(2462, 439, 15), Map.Felucca);
                FelInstance.Direction = Direction.South;
            }
        }

        public SheriffOfMinoc()
            : base(NameList.RandomName("male"), "the Sheriff of Minoc")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;

            Body = 0x190;
            Hue = Race.RandomSkinHue();
            HairItemID = 0;

            FacialHairItemID = 0x2041;
            FacialHairHue = Race.RandomHairHue();
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());

            SetWearable(new ChainCoif());
            SetWearable(new ChainChest());
            SetWearable(new ChainLegs());
            SetWearable(new Boots(), 2012);
            SetWearable(new FancyKilt(), 2012);
            SetWearable(new RingmailGloves());
            SetWearable(new BodySash(), 43);
        }

        public override void OnDoubleClick(Mobile m)
        {
            if (m is PlayerMobile)
            {
                PlayerMobile pm = m as PlayerMobile;

                if (QuestHelper.CheckDoneOnce(pm, typeof(APleaFromMinocQuest), this, false))
                {
                    if (CheckProgress(pm))
                    {
                        return;
                    }

                    AForcedSacraficeQuest2 quest = QuestHelper.GetQuest<AForcedSacraficeQuest2>(pm);

                    if (quest != null && quest.Completed)
                    {
                        quest.CompleteQuest();
                        return;
                    }

                    BaseQuest q = QuestHelper.RandomQuest(pm, new Type[] { typeof(ClearingCovetousQuest) }, this, false);

                    if (q == null)
                    {
                        q = QuestHelper.RandomQuest(pm, new Type[] { typeof(AForcedSacraficeQuest) }, this, false);

                        if (q == null)
                        {
                            q = QuestHelper.RandomQuest(pm, new Type[] { typeof(AForcedSacraficeQuest2) }, this, false);
                        }
                    }

                    if (q != null)
                    {
                        pm.CloseGump(typeof(MondainQuestGump));
                        pm.SendGump(new MondainQuestGump(q));
                    }
                    else
                    {
                        SayTo(m, 1080107, 0x3B2); // I'm sorry, I have nothing for you at this time.
                    }
                }
                else
                {
                    SayTo(m, 1080107, 0x3B2); // I'm sorry, I have nothing for you at this time.
                }
            }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m is PlayerMobile && InRange(m.Location, 5) && !InRange(oldLocation, 5))
            {
                APleaFromMinocQuest quest = QuestHelper.GetQuest<APleaFromMinocQuest>((PlayerMobile)m);

                if (quest != null)
                {
                    quest.CompleteQuest();
                }
            }
        }

        private bool CheckProgress(PlayerMobile pm)
        {
            foreach (Type t in _Quests)
            {
                BaseQuest quest = QuestHelper.GetQuest(pm, t);

                if (quest != null && !quest.Completed)
                {
                    pm.CloseGump(typeof(MondainQuestGump));
                    pm.SendGump(new MondainQuestGump(quest, MondainQuestGump.Section.InProgress, false));
                    return true;
                }
            }

            return false;
        }

        private readonly Type[] _Quests = { typeof(ClearingCovetousQuest), typeof(AForcedSacraficeQuest), typeof(AForcedSacraficeQuest2) };

        public SheriffOfMinoc(Serial serial)
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

            int version = reader.ReadInt();

            if (Map == Map.Trammel)
            {
                TramInstance = this;
            }

            if (Map == Map.Felucca)
            {
                FelInstance = this;
            }
        }
    }
}
