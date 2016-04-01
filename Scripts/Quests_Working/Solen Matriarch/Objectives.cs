using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Matriarch
{
    public class KillInfiltratorsObjective : QuestObjective
    {
        public KillInfiltratorsObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Kill 7 black/red solen infiltrators.
                return ((SolenMatriarchQuest)this.System).RedSolen ? 1054086 : 1054085;
            }
        }
        public override int MaxProgress
        {
            get
            {
                return 7;
            }
        }
        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!this.Completed)
            {
                // Black/Red Solen Infiltrators killed:
                gump.AddHtmlLocalized(70, 260, 270, 100, ((SolenMatriarchQuest)this.System).RedSolen ? 1054088 : 1054087, BaseQuestGump.Blue, false, false);
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

            bool redSolen = ((SolenMatriarchQuest)this.System).RedSolen;

            if (redSolen)
                return from is BlackSolenInfiltratorWarrior || from is BlackSolenInfiltratorQueen;
            else
                return from is RedSolenInfiltratorWarrior || from is RedSolenInfiltratorQueen;
        }

        public override void OnKill(BaseCreature creature, Container corpse)
        {
            bool redSolen = ((SolenMatriarchQuest)this.System).RedSolen;

            if (redSolen)
            {
                if (creature is BlackSolenInfiltratorWarrior || creature is BlackSolenInfiltratorQueen)
                    this.CurProgress++;
            }
            else
            {
                if (creature is RedSolenInfiltratorWarrior || creature is RedSolenInfiltratorQueen)
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
                /* You've completed your task of slaying solen infiltrators. Return to the
                * Matriarch who gave you this task.
                */
                return 1054090;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new GatherWaterConversation());
        }
    }

    public class GatherWaterObjective : QuestObjective
    {
        public GatherWaterObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Gather 8 gallons of water for the water vats of the solen ant lair.
                return 1054092;
            }
        }
        public override int MaxProgress
        {
            get
            {
                return 40;
            }
        }
        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!this.Completed)
            {
                gump.AddHtmlLocalized(70, 260, 270, 100, 1054093, BaseQuestGump.Blue, false, false); // Gallons of Water gathered:
                gump.AddLabel(70, 280, 0x64, (this.CurProgress / 5).ToString());
                gump.AddLabel(100, 280, 0x64, "/");
                gump.AddLabel(130, 280, 0x64, (this.MaxProgress / 5).ToString());
            }
            else
            {
                base.RenderProgress(gump);
            }
        }

        public override void OnComplete()
        {
            this.System.AddObjective(new ReturnAfterWaterObjective());
        }
    }

    public class ReturnAfterWaterObjective : QuestObjective
    {
        public ReturnAfterWaterObjective()
        {
        }

        public override object Message
        {
            get
            {
                // You've completed your task of gathering water. Return to the Matriarch who gave you this task.
                return 1054095;
            }
        }
        public override void OnComplete()
        {
            PlayerMobile player = this.System.From;
            bool redSolen = ((SolenMatriarchQuest)this.System).RedSolen;

            bool friend = SolenMatriarchQuest.IsFriend(player, redSolen);

            this.System.AddConversation(new ProcessFungiConversation(friend));

            if (redSolen)
                player.SolenFriendship = SolenFriendship.Red;
            else
                player.SolenFriendship = SolenFriendship.Black;
        }
    }

    public class ProcessFungiObjective : QuestObjective
    {
        public ProcessFungiObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Give the Solen Matriarch a stack of zoogi fungus to process into powder of translocation.
                return 1054098;
            }
        }
        public override void OnComplete()
        {
            if (SolenMatriarchQuest.GiveRewardTo(this.System.From))
            {
                this.System.Complete();
            }
            else
            {
                this.System.AddConversation(new FullBackpackConversation(true));
            }
        }
    }

    public class GetRewardObjective : QuestObjective
    {
        public GetRewardObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Return to the solen matriarch for your reward.
                return 1054149;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new EndConversation());
        }
    }
}