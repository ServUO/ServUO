using Server;
using System;
using Server.Items;
using Server.Services.TownCryer;

namespace Server.Engines.Quests
{
    public class ExploringTheDeepQuest : BaseQuest
    {
        /* Exploring the Deep */
        public override object Title { get { return 1154327; } }

        /*The life of a Shipwreck Salvager does seem exciting!  Visiting Hepler Paulson at the Sons of the Sea in the 
         * City of Trinsic is certainly to be an adventure!*/
        public override object Description { get { return 1158127; } }

        /* You decide against accepting the quest. */
        public override object Refuse { get { return 1158130; } }

        /* Speak to Hepler Paulson at the Sons of the Sea in Trinsic and complete the Exploring the Deep Quest. */
        public override object Uncomplete { get { return 1158131; } }

        /* You have discovered the secrets of the Wreck of the Ararat and aided in binding the Shadowlords to their tomb within!*/
        public override object Complete { get { return 1158136; } }

        public override int CompleteMessage { get { return 1156585; } } // You've completed a quest!

        public override int AcceptSound { get { return 0x2E8; } }
        public override bool DoneOnce { get { return true; } }

        public ExploringTheDeepQuest()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(typeof(ExploringTheDeedTitleDeed), 1158142)); // Uncovering the secrets of the deep...
        }

        public void CompleteQuest()
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