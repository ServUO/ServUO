using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests.Ninja
{
    public class FindEminoBeginObjective : QuestObjective
    {
        public FindEminoBeginObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Your value as a Ninja must be proven. Find Daimyo Emino and accept the test he offers.
                return 1063174;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new FindZoelConversation());
        }
    }

    public class FindZoelObjective : QuestObjective
    {
        public FindZoelObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Find Elite Ninja Zoel immediately!
                return 1063176;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new EnterCaveConversation());
        }
    }

    public class EnterCaveObjective : QuestObjective
    {
        public EnterCaveObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Enter the cave and walk through it. You will be tested as you travel along the path.
                return 1063179;
            }
        }
        public override void CheckProgress()
        {
            if (this.System.From.Map == Map.Malas && this.System.From.InRange(new Point3D(406, 1141, 0), 2))
                this.Complete();
        }

        public override void OnComplete()
        {
            this.System.AddConversation(new SneakPastGuardiansConversation());
        }
    }

    public class SneakPastGuardiansObjective : QuestObjective
    {
        private bool m_TaughtHowToUseSkills;
        public SneakPastGuardiansObjective()
        {
        }

        public bool TaughtHowToUseSkills
        {
            get
            {
                return this.m_TaughtHowToUseSkills;
            }
            set
            {
                this.m_TaughtHowToUseSkills = value;
            }
        }
        public override object Message
        {
            get
            {
                // Use your Ninja training to move invisibly past the magical guardians.
                return 1063261;
            }
        }
        public override void CheckProgress()
        {
            if (this.System.From.Map == Map.Malas && this.System.From.InRange(new Point3D(412, 1123, 0), 3))
                this.Complete();
        }

        public override void OnComplete()
        {
            this.System.AddConversation(new UseTeleporterConversation());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_TaughtHowToUseSkills = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_TaughtHowToUseSkills);
        }
    }

    public class UseTeleporterObjective : QuestObjective
    {
        public UseTeleporterObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* The special tile is known as a teleporter.
                * Step on the teleporter tile and you will be transported to a new location.
                */
                return 1063183;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new GiveZoelNoteConversation());
        }
    }

    public class GiveZoelNoteObjective : QuestObjective
    {
        public GiveZoelNoteObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Bring the note to Elite Ninja Zoel and speak with him again. 
                * He is near the cave entrance. You can hand the note to Zoel 
                * by dragging it and dropping it on his body.
                */
                return 1063185;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new GainInnInformationConversation());
        }
    }

    public class GainInnInformationObjective : QuestObjective
    {
        public GainInnInformationObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Take the Blue Teleporter Tile from Daimyo Emino's
                * house to the Abandoned Inn. Quietly look around
                * to gain information.
                */
                return 1063190;
            }
        }
        public override void CheckProgress()
        {
            Mobile from = this.System.From;

            if (from.Map == Map.Malas && from.X > 399 && from.X < 408 && from.Y > 1091 && from.Y < 1099)
                this.Complete();
        }

        public override void OnComplete()
        {
            this.System.AddConversation(new ReturnFromInnConversation());
        }
    }

    public class ReturnFromInnObjective : QuestObjective
    {
        public ReturnFromInnObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Go back through the blue teleporter and tell Daimyo Emino what you’ve overheard.
                return 1063197;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new SearchForSwordConversation());
        }
    }

    public class SearchForSwordObjective : QuestObjective
    {
        public SearchForSwordObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* Take the white teleporter and check the chests for the sword. 
                * Leave everything else behind. Avoid damage from traps you may 
                * encounter. To use a potion, make sure at least one hand is 
                * free and double click on the bottle.
                */
                return 1063200;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new HallwayWalkConversation());
        }
    }

    public class HallwayWalkObjective : QuestObjective
    {
        private bool m_StolenTreasure;
        public HallwayWalkObjective()
        {
        }

        public bool StolenTreasure
        {
            get
            {
                return this.m_StolenTreasure;
            }
            set
            {
                this.m_StolenTreasure = value;
            }
        }
        public override object Message
        {
            get
            {
                /* Walk through the hallway being careful 
                * to avoid the traps. You may be able to 
                * time the traps to avoid injury.
                */
                return 1063202;
            }
        }
        public override void OnComplete()
        {
            this.System.AddConversation(new ReturnSwordConversation());
        }

        public override void ChildDeserialize(GenericReader reader)
        {
            int version = reader.ReadEncodedInt();

            this.m_StolenTreasure = reader.ReadBool();
        }

        public override void ChildSerialize(GenericWriter writer)
        {
            writer.WriteEncodedInt((int)0); // version

            writer.Write((bool)this.m_StolenTreasure);
        }
    }

    public class ReturnSwordObjective : QuestObjective
    {
        public ReturnSwordObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Take the sword and bring it back to Daimyo Emino.
                return 1063204;
            }
        }
        public override void CheckProgress()
        {
            Mobile from = this.System.From;

            if (from.Map != Map.Malas || from.Y > 992)
                this.Complete();
        }

        public override void OnComplete()
        {
            this.System.AddConversation(new SlayHenchmenConversation());
        }
    }

    public class SlayHenchmenObjective : QuestObjective
    {
        public SlayHenchmenObjective()
        {
        }

        public override object Message
        {
            get
            {
                // Kill three henchmen.
                return 1063206;
            }
        }
        public override int MaxProgress
        {
            get
            {
                return 3;
            }
        }
        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!this.Completed)
            {
                // Henchmen killed:
                gump.AddHtmlLocalized(70, 260, 270, 100, 1063207, BaseQuestGump.Blue, false, false);
                gump.AddLabel(70, 280, 0x64, this.CurProgress.ToString());
                gump.AddLabel(100, 280, 0x64, "/");
                gump.AddLabel(130, 280, 0x64, this.MaxProgress.ToString());
            }
            else
            {
                base.RenderProgress(gump);
            }
        }

        public override void OnKill(BaseCreature creature, Container corpse)
        {
            if (creature is Henchman)
                this.CurProgress++;
        }

        public override void OnComplete()
        {
            this.System.AddConversation(new GiveEminoSwordConversation());
        }
    }

    public class GiveEminoSwordObjective : QuestObjective
    {
        public GiveEminoSwordObjective()
        {
        }

        public override object Message
        {
            get
            {
                /* You have proven your fighting skills. Bring the Sword to
                * Daimyo Emino immediately. Be sure to follow the
                * path back to the teleporter.
                */
                return 1063210;
            }
        }
        public override void OnComplete()
        {
        }
    }
}