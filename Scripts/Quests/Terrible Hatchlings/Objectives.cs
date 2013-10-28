using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Zento
{
    public class FirstKillObjective : QuestObjective
    {
        public FirstKillObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Kill 10 Deathwatch Beetle Hatchlings and return to Ansella Gryen.
                return 1063316;
            }
        }
        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!this.Completed)
            {
                // Deathwatch Beetle Hatchlings killed:
                gump.AddHtmlLocalized(70, 260, 270, 100, 1063318, 0x12DC6BF, false, false);

                gump.AddLabel(70, 280, 0x64, "0");
                gump.AddLabel(100, 280, 0x64, "/");
                gump.AddLabel(130, 280, 0x64, "10");
            }
            else
            {
                base.RenderProgress(gump);
            }
        }

        public override void OnKill(BaseCreature creature, Container corpse)
        {
            if (creature is DeathwatchBeetleHatchling)
                this.Complete();
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new SecondKillObjective());
        }
    }

    public class SecondKillObjective : QuestObjective
    {
        public SecondKillObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Great job! One less terrible hatchling in the Waste!<BR><BR>
                * 
                * Once you've killed 10 of the Deathwatch Beetle Hatchlings,
                * return to Ansella for your reward!
                */
                return 1063320;
            }
        }
        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!this.Completed)
            {
                // Deathwatch Beetle Hatchlings killed:
                gump.AddHtmlLocalized(70, 260, 270, 100, 1063318, 0x12DC6BF, false, false);

                gump.AddLabel(70, 280, 0x64, "1");
                gump.AddLabel(100, 280, 0x64, "/");
                gump.AddLabel(130, 280, 0x64, "10");
            }
            else
            {
                base.RenderProgress(gump);
            }
        }

        public override void OnKill(BaseCreature creature, Container corpse)
        {
            if (creature is DeathwatchBeetleHatchling)
            {
                this.Complete();
                this.System.AddObjective(new ThirdKillObjective(2));
            }
        }

        public override void OnRead()
        {
            if (!this.Completed)
            {
                this.Complete();
                this.System.AddObjective(new ThirdKillObjective(1));
            }
        }
    }

    public class ThirdKillObjective : QuestObjective
    {
        public ThirdKillObjective(int startingProgress)
        {
            this.CurProgress = startingProgress;
        }

        public ThirdKillObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Continue killing Deathwatch Beetle Hatchlings.
                return 1063319;
            }
        }
        public override int MaxProgress
        {
            get
            {
                return 10;
            }
        }
        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!this.Completed)
            {
                // Deathwatch Beetle Hatchlings killed:
                gump.AddHtmlLocalized(70, 260, 270, 100, 1063318, 0x12DC6BF, false, false);

                gump.AddLabel(70, 280, 0x64, this.CurProgress.ToString());
                gump.AddLabel(100, 280, 0x64, "/");
                gump.AddLabel(130, 280, 0x64, "10");
            }
            else
            {
                base.RenderProgress(gump);
            }
        }

        public override void OnKill(BaseCreature creature, Container corpse)
        {
            if (creature is DeathwatchBeetleHatchling)
                this.CurProgress++;
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new ReturnObjective());
        }
    }

    public class ReturnObjective : QuestObjective
    {
        public ReturnObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Return to Ansella Gryen for your reward.
                return 1063313;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new EndConversation());
        }
    }
}