using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Services.TownCryer;

namespace Server.Engines.Quests
{
    public class AVisitToCastleBlackthornQuest : BaseQuest
    {
        private object _Title = 1158197;

        /* A Visit to Castle Blackthorn */
        public override object Title { get { return _Title; } }

        /*It seems that Castle Blackthorn has some secrets that are worth investigating. Your history on how Blackthorn even became 
         * king is a little fuzzy. You decide a visit to Castle Blackthorn would be worthwhile.*/
        public override object Description { get { return 1158198; } }

        /* You decide against accepting the quest. */
        public override object Refuse { get { return 1158130; } }

        /* Visit Castle Blackthorn in Northern Britain. */
        public override object Uncomplete { get { return 1158199; } }

        /* As you cross the bridge Blackthorn's massive castle towers up from the terrain it sits atop.  The jet-black stone of her walls
         * are foreboding, yet invite your curiosity. Your eyes scan the courtyard and fixate on an incredibly handsome man. You've never 
         * seen such beauty and style! Their exquisitely apportioned jester suit perfectly toes the line between comedic expression and 
         * fashion. His hat is masterfully crafted with golden bells matching radiant jewelry that jingles as he swigs from a bottle. 
         * You decide to follow the jester, who is no doubt headed for the castle bar. */
        public override object Complete { get { return 1158203; } }

        public override int CompleteMessage { get { return 1156585; } } // You've completed a quest!

        public override int AcceptSound { get { return 0x2E8; } }
        public override bool DoneOnce { get { return true; } }

        public AVisitToCastleBlackthornQuest()
        {
            AddObjective(new InternalObjective());

            AddReward(new BaseReward(1158200)); // A step closer to understanding the history of Blackthorn's Rise to the Throne.
        }

        public static void CheckLocation(PlayerMobile pm, Point3D oldLocation)
        {
            var quest = QuestHelper.GetQuest<AVisitToCastleBlackthornQuest>(pm);

            if (quest != null)
            {
                quest.OnCompleted();
                TownCryerSystem.CompleteQuest(quest.Owner, 1158202, quest.Complete, 0x61B);

                quest.Objectives[0].CurProgress++;
                quest.GiveRewards();
            }
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