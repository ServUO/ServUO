using Server;
using System;
using Server.Items;
using Server.Services.TownCryer;

namespace Server.Engines.Quests
{
    public class HuntmastersChallengeQuest : BaseQuest
    {
        /* Huntmaster's Challenge */
        public override object Title { get { return 1155726; } }

        /*Each month the Ranger's Guild in Skara Brae hosts a contest to see who can hunt Britannia's 
         * largest creatures! Visit the Skara Brae Ranger's Guild to learn more!*/
        public override object Description { get { return 1158132; } }

        /* You decide against accepting the quest. */
        public override object Refuse { get { return 1158130; } }

        /* Visit the Skara Brae Ranger's Guild and participate in the Huntmaster's Challenge */
        public override object Uncomplete { get { return 1158133; } }

        /* You have braved the wilds of Britannia and slayed a mighty beast! You have meticulously documented your kill 
         * and submitted it to the Ranger's Guild for evaluation. If luck was on your side you may indeed have 
         * the largest quarry for the month...or maybe not. Alas, your bravery has earned you the well deserved title
         * of Hunter! May you go fearlessly into the wilderness in search of your next big kill! Well done! */
        public override object Complete { get { return 1158378; } }

        public override int CompleteMessage { get { return 1156585; } } // You've completed a quest!

        public override int AcceptSound { get { return 0x2E8; } }
        public override bool DoneOnce { get { return true; } }

        public HuntmastersChallengeQuest()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(typeof(HuntmastersQuestRewardTitleDeed), 1158139)); // A Reward Title Deed
        }

        public void CompleteChallenge()
        {
            OnCompleted();
            Objectives[0].CurProgress++;
            TownCryerSystem.CompleteQuest(Owner, this);
            GiveRewards();
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription { get { return Quest.Uncomplete; } }

            public InternalObjective()
                : base(1)
            {
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
            }
        }
    }
}