using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Ambitious
{
    public class KillQueensObjective : QuestObjective
    {
        public override object Message =>
                // Kill 5 red/black solen queens.
                ((AmbitiousQueenQuest)System).RedSolen ? 1054062 : 1054063;
        public override int MaxProgress => 5;
        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!Completed)
            {
                // Red/Black Solen Queens killed:
                gump.AddHtmlLocalized(70, 260, 270, 100, ((AmbitiousQueenQuest)System).RedSolen ? 1054064 : 1054065, BaseQuestGump.Blue, false, false);
                gump.AddLabel(70, 280, 0x64, CurProgress.ToString());
                gump.AddLabel(100, 280, 0x64, "/");
                gump.AddLabel(130, 280, 0x64, MaxProgress.ToString());
            }
            else
            {
                base.RenderProgress(gump);
            }
        }

        public override bool IgnoreYoungProtection(Mobile from)
        {
            if (Completed)
                return false;

            bool redSolen = ((AmbitiousQueenQuest)System).RedSolen;

            if (redSolen)
                return from is RedSolenQueen;
            else
                return from is BlackSolenQueen;
        }

        public override void OnKill(BaseCreature creature, Container corpse)
        {
            bool redSolen = ((AmbitiousQueenQuest)System).RedSolen;

            if (redSolen)
            {
                if (creature is RedSolenQueen)
                    CurProgress++;
            }
            else
            {
                if (creature is BlackSolenQueen)
                    CurProgress++;
            }
        }

        public override void OnComplete()
        {
            System.AddObjective(new ReturnAfterKillsObjective());
        }
    }

    public class ReturnAfterKillsObjective : QuestObjective
    {
        public override object Message =>
                /* You've completed your task of slaying solen queens. Return to
* the ambitious queen who asked for your help.
*/
                1054067;
        public override void OnComplete()
        {
            System.AddConversation(new GatherFungiConversation());
        }
    }

    public class GatherFungiObjective : QuestObjective
    {
        public override object Message =>
                /* Gather zoogi fungus until you have 50 of them, then give them
* to the ambitious queen you are helping.
*/
                1054069;
        public override void OnComplete()
        {
            System.AddConversation(new EndConversation());
        }
    }

    public class GetRewardObjective : QuestObjective
    {
        private bool m_BagOfSending;
        private bool m_PowderOfTranslocation;
        private bool m_Gold;
        public GetRewardObjective(bool bagOfSending, bool powderOfTranslocation, bool gold)
        {
            m_BagOfSending = bagOfSending;
            m_PowderOfTranslocation = powderOfTranslocation;
            m_Gold = gold;
        }

        public GetRewardObjective()
        {
        }

        public override object Message =>
                // Return to the ambitious solen queen for your reward.
                1054148;
        public bool BagOfSending
        {
            get
            {
                return m_BagOfSending;
            }
            set
            {
                m_BagOfSending = value;
            }
        }
        public bool PowderOfTranslocation
        {
            get
            {
                return m_PowderOfTranslocation;
            }
            set
            {
                m_PowderOfTranslocation = value;
            }
        }
        public bool Gold
        {
            get
            {
                return m_Gold;
            }
            set
            {
                m_Gold = value;
            }
        }
        public override void OnComplete()
        {
            System.AddConversation(new End2Conversation());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            m_BagOfSending = reader.ReadBool();
            m_PowderOfTranslocation = reader.ReadBool();
            m_Gold = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt(0); // version

            writer.Write(m_BagOfSending);
            writer.Write(m_PowderOfTranslocation);
            writer.Write(m_Gold);
        }
    }
}
