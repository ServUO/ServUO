using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Ambitious
{
    public class KillQueensObjective : QuestObjective
    {
        public KillQueensObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Kill 5 red/black solen queens.
                return ((AmbitiousQueenQuest)this.System).RedSolen ? 1054062 : 1054063;
            }
        }
        public override int MaxProgress
        {
            get
            {
                return 5;
            }
        }
        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!this.Completed)
            {
                // Red/Black Solen Queens killed:
                gump.AddHtmlLocalized(70, 260, 270, 100, ((AmbitiousQueenQuest)this.System).RedSolen ? 1054064 : 1054065, BaseQuestGump.Blue, false, false);
                gump.AddLabel(70, 280, 0x64, this.CurProgress.ToString());
                gump.AddLabel(100, 280, 0x64, "/");
                gump.AddLabel(130, 280, 0x64, this.MaxProgress.ToString());
            }
            else
            {
                base.RenderProgress(gump);
            }
        }

        public override bool IgnoreYoungProtection(Mobile from)
        {
            if (this.Completed)
                return false;

            bool redSolen = ((AmbitiousQueenQuest)this.System).RedSolen;

            if (redSolen)
                return from is RedSolenQueen;
            else
                return from is BlackSolenQueen;
        }

        public override void OnKill(BaseCreature creature, Container corpse)
        {
            bool redSolen = ((AmbitiousQueenQuest)this.System).RedSolen;

            if (redSolen)
            {
                if (creature is RedSolenQueen)
                    this.CurProgress++;
            }
            else
            {
                if (creature is BlackSolenQueen)
                    this.CurProgress++;
            }
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new ReturnAfterKillsObjective());
        }
    }

    public class ReturnAfterKillsObjective : QuestObjective
    {
        public ReturnAfterKillsObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* You've completed your task of slaying solen queens. Return to
                * the ambitious queen who asked for your help.
                */
                return 1054067;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new GatherFungiConversation());
        }
    }

    public class GatherFungiObjective : QuestObjective
    {
        public GatherFungiObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Gather zoogi fungus until you have 50 of them, then give them
                * to the ambitious queen you are helping.
                */
                return 1054069;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new EndConversation());
        }
    }

    public class GetRewardObjective : QuestObjective
    {
        private bool m_BagOfSending;
        private bool m_PowderOfTranslocation;
        private bool m_Gold;
        public GetRewardObjective(bool bagOfSending, bool powderOfTranslocation, bool gold)
        {
            this.m_BagOfSending = bagOfSending;
            this.m_PowderOfTranslocation = powderOfTranslocation;
            this.m_Gold = gold;
        }

        public GetRewardObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Return to the ambitious solen queen for your reward.
                return 1054148;
            }
        }
        public bool BagOfSending
        {
            get
            {
                return this.m_BagOfSending;
            }
            set
            {
                this.m_BagOfSending = value;
            }
        }
        public bool PowderOfTranslocation
        {
            get
            {
                return this.m_PowderOfTranslocation;
            }
            set
            {
                this.m_PowderOfTranslocation = value;
            }
        }
        public bool Gold
        {
            get
            {
                return this.m_Gold;
            }
            set
            {
                this.m_Gold = value;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new End2Conversation());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_BagOfSending = reader.ReadBool();
            this.m_PowderOfTranslocation = reader.ReadBool();
            this.m_Gold = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_BagOfSending);
            writer.Write((bool)this.m_PowderOfTranslocation);
            writer.Write((bool)this.m_Gold);
        }
    }
}